using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace SpaceAlertSolver;

/* ----------------
 *Adjusted rulings: 
 * -Few threats are missing
 * -No heroic cards
 * -No hand card limit
 * 
 * Questionable stuff:
 * Seeker counts dead players when looking for target
 * 
 * #TODO
 * Add menu class
 * Option to remove commons from threat pool
 * Add verbose replay mode
 * Remove some double checks from GetDistance and DealDamage
*/


public sealed class Game : IGame
{
    public const int ROCKET_DAMAGE = 3;

    public static List<int> Scores = new();
    private static readonly int[] _obsBonus = new int[] { 0, 1, 2, 3, 5, 7, 9, 11, 13, 15, 17 };

    internal readonly Ship ship;
    public Player[] players = null!;
    public ImmutableArray<Trajectory> trajectories;
    internal readonly RefList<Threat> Threats = new();
    double score;
    private bool _didComputerThisPhase;
    bool gameover;

    int exSlain, exSurvived, inSlain, inSurvived;

    private int _totalObservationPoints;
    private int _maxObservationThisPhase;
    private int _observationThisTurn;

    double scoreMultiplier;
    double scoreAddition;

    public int ScoutBonus;

    private readonly List<SimulationStep> _simulationStack = new();

    public Game()
    {
        ship = new(this);
    }

    public void Init(Game other)
    {
        ship.Init(other.ship);
        InitPlayers(other.players);
        trajectories = other.trajectories;
        Threats.Clear();
        Threats.AddRange(other.Threats);
        for (int i = 0; i < Threats.Count; i++)
        {
            Threats[i].Game = this;
        }

        score = other.score;
        _didComputerThisPhase = other._didComputerThisPhase;
        gameover = other.gameover;
        exSlain = other.exSlain;
        exSurvived = other.exSurvived;
        inSlain = other.inSlain;
        inSurvived = other.inSurvived;

        _totalObservationPoints = other._totalObservationPoints;
        _maxObservationThisPhase = other._maxObservationThisPhase;
        _observationThisTurn = other._observationThisTurn;

        scoreMultiplier = 1.0;
        scoreAddition = 0.0;

        ScoutBonus = other.ScoutBonus;

        _simulationStack.Clear();
        _simulationStack.AddRange(other._simulationStack);
    }

    public void Init(Player[] players, ImmutableArray<Trajectory> trajectories, ImmutableArray<Event> events)
    {
        ship.Init();
        InitPlayers(players);
        this.trajectories = trajectories;
        Threats.Clear();
        score = 0.0;
        _didComputerThisPhase = false;
        gameover = false;
        exSlain = 0;
        inSlain = 0;
        exSurvived = 0;
        inSurvived = 0;
        _totalObservationPoints = 0;
        _maxObservationThisPhase = 0;
        _observationThisTurn = 0;
        scoreMultiplier = 1.0;
        scoreAddition = 0.0;
        ScoutBonus = 0;
        InitSimulationStack(players.Length, events);
    }

    private void InitPlayers(Player[] other)
    {
        if (players == null || players.Length != other.Length)
            players = new Player[other.Length];

        other.CopyTo(players, 0);
    }

    private void InitSimulationStack(int numPlayers, ImmutableArray<Event> events)
    {
        const int NUM_NORMAL_TURNS = 12;

        _simulationStack.Clear();

        _simulationStack.Add(SimulationStep.NewCleanThreatsStep());
        _simulationStack.Add(SimulationStep.NewCreateMovesStep());
        _simulationStack.Add(SimulationStep.NewCleanThreatsStep());
        _simulationStack.Add(SimulationStep.NewProcessDamageStep());
        _simulationStack.Add(SimulationStep.NewRocketUpdateStep());

        int eventIndex = events.Length - 1;
        for (int i = NUM_NORMAL_TURNS; i > 0; i--)
        {
            _simulationStack.Add(SimulationStep.NewCleanThreatsStep());
            _simulationStack.Add(SimulationStep.NewCreateMovesStep());
            _simulationStack.Add(SimulationStep.NewCleanThreatsStep());
            _simulationStack.Add(SimulationStep.NewProcessDamageStep());
            _simulationStack.Add(SimulationStep.NewRocketUpdateStep());

            bool startNewObservationPhase = (i == 12 || i == 7 || i == 3);
            _simulationStack.Add(SimulationStep.NewObservationUpdateStep(startNewObservationPhase));

            for (int j = numPlayers - 1; j >= 0; j--)
            {
                _simulationStack.Add(SimulationStep.NewPlayerActionStep(j));
            }

            if (i == 3 || i == 6 || i == 10)
                _simulationStack.Add(SimulationStep.NewCheckComputerStep());
            else if (i == 4 || i == 8)
                _simulationStack.Add(SimulationStep.NewResetComputerStep());

            if (eventIndex >= 0 && events[eventIndex].Turn == i)
            {
                _simulationStack.Add(SimulationStep.NewThreatSpawnStep(events[eventIndex]));
                eventIndex--;
            }

            _simulationStack.Add(SimulationStep.NewTurnStartStep());
        }
    }

    public double Simulate()
    {
        while (!gameover && _simulationStack.Count > 0)
        {
            int index = _simulationStack.Count - 1;
            SimulationStep simulationStep = _simulationStack[index];
            HandleSimulationStep(in simulationStep);
            _simulationStack.RemoveAt(index);
        }

        return CalculateScore();
    }

    private void HandleSimulationStep(in SimulationStep simulationStep)
    {
        switch (simulationStep.Type)
        {
            case SimulationStepType.TurnStart:
                TurnStart();
                break;
            case SimulationStepType.ResetComputer:
                ResetComputer();
                break;
            case SimulationStepType.CheckComputer:
                CheckComputer();
                break;
            case SimulationStepType.PlayerAction:
                PlayerAction(simulationStep.PlayerIndex);
                break;
            case SimulationStepType.ObservationUpdate:
                ObservationUpdate(simulationStep.StartNewObservationPhase);
                break;
            case SimulationStepType.RocketUpdate:
                RocketUpdate();
                break;
            case SimulationStepType.ProcessDamage:
                ProcessDamage();
                break;
            case SimulationStepType.CleanThreats:
                CleanThreats();
                break;
            case SimulationStepType.CreateMoves:
                CreateMoves();
                break;
            case SimulationStepType.MoveThreat:
                MoveThreat(simulationStep.ThreatId, simulationStep.Speed);
                break;
            case SimulationStepType.SpawnThreat:
                SpawnThreat(in simulationStep);
                break;
            case SimulationStepType.ActX:
                Threats[simulationStep.ThreatIndex].ActX();
                break;
            case SimulationStepType.ActY:
                Threats[simulationStep.ThreatIndex].ActY();
                break;
            case SimulationStepType.ActZ:
                Threats[simulationStep.ThreatIndex].ActZ();
                break;
            case SimulationStepType.DealExternalDamage:
                HandleExternalDamageStep(simulationStep.Zone, simulationStep.Damage);
                break;
            default:
                throw new UnreachableException();
        }
    }

    public void DestroyShip()
    {
        gameover = true;
    }

    public void DealExternalDamage(int zone, int damage)
    {
        _simulationStack.Add(SimulationStep.NewDealExternalDamageStep(zone, damage));
    }

    private void HandleExternalDamageStep(int zone, int damage)
    {
        BranchShieldFull(zone);
        gameover |= ship.DealExternalDamage(zone, damage);
    }

    private void PlayerAction(int playerIndex)
    {
        ref Player player = ref players[playerIndex];
        if (!player.Alive)
            return;

        Act action = player.GetNextAction();
        if (player.Position == Position.Space)
        {
            if (action == Act.Fight)
                UseInterceptors();
            else
            {
                player.ReturnFromSpace();
                ship.InterceptorReady = true;
                player.DelayCurrent();
            }
            return;
        }

        switch (action)
        {
            case Act.Empty:
                break;
            case Act.Right:
                player.TryMoveRight();
                break;
            case Act.Left:
                player.TryMoveLeft();
                break;
            case Act.Lift:
                if (ship.LiftWillDelay(player.Position))
                    player.DelayNext();
                ship.UseLift(player.Position);
                player.TryTakeElevator();
                break;
            case Act.A:
                break;
            case Act.B:
                break;
            case Act.C:
                break;
            case Act.Fight:
                throw new NotImplementedException();
            case Act.HeroicTopLeft:
                throw new NotImplementedException();
            case Act.HeroicTopMiddle:
                throw new NotImplementedException();
            case Act.HeroicTopRight:
                throw new NotImplementedException();
            case Act.HeroicDownLeft:
                throw new NotImplementedException();
            case Act.HeroicDownMiddle:
                throw new NotImplementedException();
            case Act.HeroicDownRight:
                throw new NotImplementedException();
            case Act.HeroicA:
                throw new NotImplementedException();
            case Act.HeroicB:
                throw new NotImplementedException();
            case Act.HeroicFight:
                throw new NotImplementedException();
            default:
                throw new UnreachableException();
        }
    }

    private void UseInterceptors()
    {
        int target = -1;
        for (int i = 0; i < Threats.Count; i++)
        {
            int distance = Threats[i].GetDistance(DamageSource.Interceptors);
            if (distance >= Trajectory.RANGE_2_START)
                continue;

            if (target == -2)
                Threats[i].DealDamage(DamageSource.Interceptors, 1);
            else if (target == -1)
                target = i;
            else
            {
                Debug.Assert(target >= 0);
                Threats[target].DealDamage(DamageSource.Interceptors, 1);
                Threats[i].DealDamage(DamageSource.Interceptors, 1);
                target = -2;
            }
        }
        if (target >= 0)
            Threats[target].DealDamage(DamageSource.Interceptors, 3);
    }

    private void TurnStart()
    {
        ship.OnTurnStart();
        Debug.Assert(ship.Damage[0] < 7, "Game should have ended if ship was broken");
        Debug.Assert(ship.Damage[1] < 7, "Game should have ended if ship was broken");
        Debug.Assert(ship.Damage[2] < 7, "Game should have ended if ship was broken");
    }

    private void ResetComputer()
    {
        _didComputerThisPhase = false;
    }

    private void CheckComputer()
    {
        if (_didComputerThisPhase)
            return;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Position.IsInShip())
                players[i].DelayNext();
        }
    }

    private void ObservationUpdate(bool startNewPhase)
    {
        _maxObservationThisPhase = Math.Max(_maxObservationThisPhase, _observationThisTurn);
        _observationThisTurn = 0;
        if (startNewPhase)
        {
            _totalObservationPoints += _obsBonus[_maxObservationThisPhase];
            _maxObservationThisPhase = 0;
        }
    }

    private void RocketUpdate()
    {
        if (ship.RocketReady)
        {
            int shortestDistance = Trajectory.RANGE_3_START;
            int targetIndex = -1;
            for (int i = 0; i < Threats.Count; i++)
            {
                if (!Threats[i].IsExternal)
                    continue;

                int distance = Threats[i].GetDistance(DamageSource.Rocket);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    targetIndex = i;
                }
            }
            if (targetIndex != -1)
            {
                Threats[targetIndex].DealDamage(DamageSource.Rocket, ROCKET_DAMAGE);
            }
        }
        ship.MoveRockets();
    }

    private void ProcessDamage()
    {
        for (int i = 0; i < Threats.Count; i++)
        {
            if (!Threats[i].IsExternal || Threats[i].Damage > 0)
                Threats[i].ProcessDamage();
        }
    }

    private void CleanThreats()
    {
        for (int i = Threats.Count - 1; i >= 0; i--)
        {
            ref Threat threat = ref Threats[i];
            if (threat.Beaten)
            {
                if (threat.Alive)
                {
                    score += threat.ScoreLose;
                    if (threat.IsExternal)
                        exSurvived++;
                    else
                        inSurvived++;
                }
                else
                {
                    score += threat.ScoreWin;
                    if (threat.IsExternal)
                        exSlain++;
                    else
                        inSlain++;
                }
                threat.OnBeaten();
                Threats.RemoveAt(i);
            }
        }
    }

    private void CreateMoves()
    {
        for (int i = Threats.Count - 1; i >= 0; i--)
        {
            int speed = Math.Min(Threats[i].Speed, Threats[i].Distance);
            Debug.Assert(speed > 0, "If the threat has no more distance to travel it should not exist");
            _simulationStack.Add(SimulationStep.NewMoveThreatStep(i, speed));
        }
    }

    private void MoveThreat(int threatId, int speed)
    {
        int newPos = Threats[threatId].Distance - speed;
        for (int d = newPos; d < Threats[threatId].Distance; d++)
        {
            switch (trajectories[Threats[threatId].Zone].actions[d])
            {
                case 0:
                    break;
                case 1:
                    _simulationStack.Add(SimulationStep.NewActXStep(threatId));
                    break;
                case 2:
                    _simulationStack.Add(SimulationStep.NewActYStep(threatId));
                    break;
                case 3:
                    _simulationStack.Add(SimulationStep.NewActZStep(threatId));
                    break;
                default:
                    throw new UnreachableException();
            }
        }
        Threats[threatId].Distance = newPos;
        if (newPos <= 0)
        {
            Threats[threatId].Beaten = true;
            Debug.Assert(Threats[threatId].Alive, "Assuming that Alive does not need to be set");
        }
    }

    private void SpawnThreat(in SimulationStep simulationStep)
    {
        Threat threat = ThreatFactory.Instance.ThreatsById[simulationStep.ThreatId];
        threat.Distance = trajectories[threat.Zone].maxDistance;
        threat.Game = this;
        Threats.Add(in threat);
    }

    double CalculateScore()
    {
        //Count damage
        int highestDamage = 0;
        for (int i = 0; i < 3; i++)
        {
            score -= ship.Damage[i];
            highestDamage = Math.Max(ship.Damage[i], highestDamage);
        }
        score -= highestDamage;

        //Count dead crew
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].Alive)
                score -= 2;
        }
        //Count dead androids
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].AndroidState == AndroidState.Disabled)
                score--;
        }
        if (ship.AndroidTopRight == AndroidState.Disabled)
            score--;
        if (ship.AndroidBottomLeft == AndroidState.Disabled)
            score--;

        //Observation bonus
        if (!gameover)
            score += _totalObservationPoints;

        //Gameover penalty
        if (gameover)
            score = score - 200 + CountRemainingTurnStarts();

        Scores.Add((int)score);

        return scoreMultiplier * score + scoreAddition;
    }

    private int CountRemainingTurnStarts()
    {
        int count = 0;
        for (int i = 0; i < _simulationStack.Count; i++)
        {
            count += _simulationStack[i].Type == SimulationStepType.TurnStart ? 1 : 0;
        }
        return count;
    }

    /**
     * <summary>Branches the execution for the two possibilities of the defect</summary>
     * <remarks>Adds a defect to the ship</remarks>
     * <param name="zone">The zone in which the defect is</param>
     * <param name="defect">The defect type</param>
     */
    void Branch(int zone, Defects defect)
    {
        Debug.Assert(zone >= 0 && zone < 3
            && ship.DefectStates[zone,(int)defect] == DefectState.undetermined);

        double chanceOfBreaking = ship.ChanceOfDefect(zone);
        if (chanceOfBreaking < 1.0)
        {
            Game branch = GamePool.GetGame();
            branch.Init(this);
            branch.ship.NotDefect(zone, defect);

            scoreAddition += branch.Simulate() * (1.0 - chanceOfBreaking) * scoreMultiplier;
            scoreMultiplier *= chanceOfBreaking;
            GamePool.FreeGame(branch);
        }
        ship.AddDefect(zone, defect);
    }

    /**
     * <summary>Branches the execution if the defect is undetermined</summary>
     * <remarks>Adds a defect to the ship</remarks>
     * <param name="zone">The zone in which the defect is</param>
     * <param name="defect">The defect type</param>
     */
    public void BranchConditional(int zone, Defects defect)
    {
        Debug.Assert(zone >= 0 && zone < 3);

        if (ship.DefectStates[zone,(int)defect] == DefectState.undetermined)
        {
            Branch(zone, defect);
        }
    }

    public void BranchReactorFull(int zone)
    {
        Debug.Assert(zone >= 0 && zone < 3);

        if (ship.Reactors[zone] == ship.ReactorsCap[zone])
        {
            BranchConditional(zone, Defects.reactor);
        }
    }

    public void BranchShieldFull(int zone)
    {
        Debug.Assert(zone >= 0 && zone < 3);

        if (ship.Shields[zone] == ship.ShieldsCap[zone])
        {
            BranchConditional(zone, Defects.shield);
        }
    }

    void ApplyStatusEffect(ref Player p)
    {
        //Check status effect
        if (ship.StationStatus[p.Position.PositionIndex] != 0)
        {
            // Delay
            if ((ship.StationStatus[p.Position.PositionIndex] & 1) == 1)
                p.DelayNext();
            // Kill
            else if ((ship.StationStatus[p.Position.PositionIndex] & 2) == 2)
                p.Kill();
        }
    }

    private enum SimulationStepType
    {
        TurnStart,
        ResetComputer,
        CheckComputer,
        PlayerAction,
        ObservationUpdate,
        RocketUpdate,
        ProcessDamage,
        CleanThreats,
        CreateMoves,
        MoveThreat,
        SpawnThreat,
        ActX,
        ActY,
        ActZ,
        DealExternalDamage,
    }

    private readonly struct SimulationStep
    {
        public readonly SimulationStepType Type;
        public int PlayerIndex => _value1;
        public int ThreatIndex => _value1;
        public int ThreatId => _value1;
        public int Damage => _value1;
        public int Speed => _value2;
        public int Zone => _value2;
        public bool IsExternal => _bool1;
        public bool StartNewObservationPhase => _bool1;

        private readonly int _value1, _value2;
        private readonly bool _bool1;

        private SimulationStep(SimulationStepType type, int value1 = default, int value2 = default, bool bool1 = default)
        {
            Type = type;
            _value1 = value1;
            _value2 = value2;
            _bool1 = bool1;
        }

        public static SimulationStep NewCreateMovesStep() => new(SimulationStepType.CreateMoves);

        public static SimulationStep NewMoveThreatStep(int threatId, int speed) => new(SimulationStepType.MoveThreat, value1: threatId, value2: speed);

        public static SimulationStep NewCleanThreatsStep() => new(SimulationStepType.CleanThreats);

        public static SimulationStep NewProcessDamageStep() => new(SimulationStepType.ProcessDamage);

        public static SimulationStep NewRocketUpdateStep() => new(SimulationStepType.RocketUpdate);

        public static SimulationStep NewPlayerActionStep(int playerIndex) => new(SimulationStepType.PlayerAction, value1: playerIndex);

        public static SimulationStep NewTurnStartStep() => new(SimulationStepType.TurnStart);

        public static SimulationStep NewThreatSpawnStep(int creatureId, int zone, bool isExternal)
            => new(SimulationStepType.SpawnThreat, value1:creatureId, value2: zone, bool1: isExternal);

        public static SimulationStep NewThreatSpawnStep(in Event @event) => NewThreatSpawnStep(@event.CreatureId, @event.Zone, @event.IsExternal);

        public static SimulationStep NewResetComputerStep() => new(SimulationStepType.ResetComputer);

        public static SimulationStep NewCheckComputerStep() => new(SimulationStepType.CheckComputer);

        public static SimulationStep NewObservationUpdateStep(bool startNewPhase) => new(SimulationStepType.ObservationUpdate, bool1: startNewPhase);

        public static SimulationStep NewActXStep(int threatId) => new(SimulationStepType.ActX, value1: threatId);

        public static SimulationStep NewActYStep(int threatId) => new(SimulationStepType.ActY, value1: threatId);

        public static SimulationStep NewActZStep(int threatId) => new(SimulationStepType.ActZ, value1: threatId);

        public static SimulationStep NewDealExternalDamageStep(int zone, int damage) => new(SimulationStepType.DealExternalDamage, value1: damage, value2: zone);
    }
}

public readonly record struct Event(bool IsExternal, int Turn, int Zone, int CreatureId);
