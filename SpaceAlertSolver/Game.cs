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
    public static List<int> Scores = new();
    private static readonly int[] _obsBonus = new int[] { 0, 1, 2, 3, 5, 7, 9, 11, 13, 15, 17 };
    public const int NUM_PHASES = 3;

    internal readonly Ship ship;
    public Player[] players = null!;
    public ImmutableArray<Trajectory> trajectories;
    public readonly List<ExThreat> exThreats = new();
    public readonly List<InThreat> inThreats = new();
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
        exThreats.Clear();
        exThreats.AddRange(other.exThreats.Select(t => t.Clone(this)));
        inThreats.Clear();
        inThreats.AddRange(other.inThreats.Select(t => t.Clone(this)));

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
        exThreats.Clear();
        inThreats.Clear();
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

        _simulationStack.Add(SimulationStep.NewCreateMovesStep());
        _simulationStack.Add(SimulationStep.NewCleanThreatsStep());
        _simulationStack.Add(SimulationStep.NewCreateProcessStepsStep());
        _simulationStack.Add(SimulationStep.NewRocketUpdateStep());

        int eventIndex = events.Length - 1;
        for (int i = NUM_NORMAL_TURNS; i > 0; i--)
        {
            _simulationStack.Add(SimulationStep.NewCreateMovesStep());
            _simulationStack.Add(SimulationStep.NewCleanThreatsStep());
            _simulationStack.Add(SimulationStep.NewCreateProcessStepsStep());
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

            if (events[eventIndex].Turn == i)
            {
                eventIndex--;
                _simulationStack.Add(SimulationStep.NewThreatSpawnStep(events[eventIndex]));
            }

            _simulationStack.Add(SimulationStep.NewTurnStartStep());
        }
    }

    internal ref Player GetCurrentTurnPlayer()
    {
        return ref players[actions_player_i];
    }

    public double Simulate()
    {
        while (_simulationStack.Count > 0)
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
                break;
            case SimulationStepType.ObservationUpdate:
                ObservationUpdate(simulationStep.StartNewObservationPhase);
                break;
            case SimulationStepType.RocketUpdate:
                RocketUpdate();
                break;
            case SimulationStepType.CreateProcessSteps:
                break;
            case SimulationStepType.InternalTurnEnd:
                break;
            case SimulationStepType.ExternalDamage:
                break;
            case SimulationStepType.CleanThreats:
                CleanThreats();
                break;
            case SimulationStepType.CreateMoves:
                break;
            case SimulationStepType.MoveThreat:
                break;
            case SimulationStepType.SpawnThreat:
                SpawnThreat(in simulationStep);
                break;
            default:
                throw new UnreachableException();
        }
    }

    public void DealExternalDamage(int zone, int damage)
    {
        BranchShieldFull(zone);
        ship.DealExternalDamage(zone, damage);
    }

    private void TurnStart()
    {
        ship.OnTurnStart();
        for (int i = 0; i < 3; i++)
        {
            if (ship.Damage[i] >= 7)
                gameover = true;
        }
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
            int distance = 99;
            ExThreat? target = null;
            for (int i = 0; i < exThreats.Count; i++)
            {
                ExThreat et = exThreats[i];
                int dist = et.GetDistance(2, ExDmgSource.rocket);
                if (dist < distance)
                {
                    target = et;
                    distance = dist;
                }
            }
            target?.DealDamage(3, 2, ExDmgSource.rocket);
        }
        ship.MoveRockets();
    }

    void CleanThreats()
    {
        for (int i = inThreats.Count - 1; i >= 0; i--)
        {
            if (inThreats[i].beaten)
            {
                if (inThreats[i].alive)
                {
                    score += inThreats[i].scoreLose;
                    inSurvived++;
                }
                else
                {
                    score += inThreats[i].scoreWin;
                    inSlain++;
                }
                inThreats.RemoveAt(i);
            }
        }

        for (int i = exThreats.Count - 1; i >= 0; i--)
        {
            if (exThreats[i].beaten)
            {
                if (exThreats[i].alive)
                {
                    score += exThreats[i].scoreLose;
                    exSurvived++;
                }
                else
                {
                    score += exThreats[i].scoreWin;
                    exSlain++;
                }
                exThreats.RemoveAt(i);
            }
        }
    }

    private void SpawnThreat(in SimulationStep simulationStep)
    {
        if (simulationStep.IsExternal)
            exThreats.Add(ThreatFactory.SummonEx(simulationStep.CreatureId, trajectories[simulationStep.Zone], simulationStep.Zone, this));
        else
            inThreats.Add(ThreatFactory.SummonIn(simulationStep.CreatureId, trajectories[3], this));
    }

    void PlayerActions(ref Player p)
    {
        int t = turn - 1;

        //Exit if dead
        if (!p.Alive)
            return;

        //Check interceptor controls
        if (p.InIntercept)
        {
            Debug.Assert(!ship.InterceptorReady, "Interceptor cannot be ready when player is in it");

            if (action == Act.Fight)
            {
                //Keep fighting
                InterceptorDamage(ref p);
                return;
            }

            //Return
            p.InIntercept = false;
            ship.InterceptorReady = true;
            p.ReturnFromSpace();
            ApplyStatusEffect(ref p);
            p.DelayCurrent();
            return;
        }

        //Perform action
        switch (action)
        {
            case Act.Left:
                {
                    Position oldPosition = p.Position;
                    p.TryMoveLeft();
                    if (p.Position != oldPosition)
                        ApplyStatusEffect(ref p);
                    break;
                }
            case Act.Right:
                {
                    Position oldPosition = p.Position;
                    p.TryMoveRight();
                    if (p.Position != oldPosition)
                        ApplyStatusEffect(ref p);
                    break;
                }
            case Act.Lift:
                PlayerActionLift(ref p);
                break;
            case Act.A:
                PlayerActionA(ref p);
                break;
            case Act.B:
                PlayerActionB(ref p);
                break;
            case Act.C:
                PlayerActionC(ref p);
                break;
            //Empty for now
            case Act.Fight:
                PlayerActionFight(ref p);
                break;
            case Act.Empty:
                break;
        }
    }

    void PlayerActionLift(ref Player p)
    {
        int t = turn - 1;

        // branching
        if (t < 11 && p.PeekNextAction() != Act.Empty) // no branching needed if nothing to be delayed
        {
            BranchConditional(p.Position.Zone, Defects.lift);
        }

        //Check if elevator was used
        if (ship.LiftWillDelay(p.Position))
            p.DelayNext();
        ship.UseLift(p.Position.Zone);
        //Move
        p.TryTakeElevator();
        ApplyStatusEffect(ref p);
    }

    void PlayerActionA(ref Player p)
    {
        //Check if can fire
        if (!ship.CanFireCannon(p.Position))
            return;

        if (p.Position.IsTop())
        {
            //Main guns
            //Drain energy
            if (ship.Reactors[p.Position.Zone] > 0)
            {
                BranchReactorFull(p.Position.Zone);

                //Find target
                int target = GetTargetEx(p.Position.Zone, 3, ExDmgSource.laser);

                if (target != -1)
                {
                    BranchConditional(p.Position.Zone, Defects.weapontop);

                    exThreats[target].DealDamage(ship.LaserDamage[p.Position.Zone], 3, ExDmgSource.laser);
                }

                ship.Reactors[p.Position.Zone]--;
            }
        }
        else
        {
            //Secondary guns
            //Impulse cannon
            if (p.Position.IsMiddle())
            {
                //Drain energy
                if (ship.Reactors[p.Position.Zone] > 0)
                {
                    BranchReactorFull(p.Position.Zone);
                    BranchConditional(p.Position.Zone, Defects.weaponbot);

                    ship.Reactors[p.Position.Zone]--;
                    foreach (ExThreat et in exThreats)
                        et.DealDamage(1, ship.PlasmaDamage[p.Position.Zone], ExDmgSource.impulse);
                }
            }
            //Plasma cannon
            else
            {
                //Find target
                int target = GetTargetEx(p.Position.Zone, 3, ExDmgSource.plasma);
                //Deal damage
                if (target != -1)
                {
                    BranchConditional(p.Position.Zone, Defects.weaponbot);
                    exThreats[target].DealDamage(ship.PlasmaDamage[p.Position.Zone], 3, ExDmgSource.plasma);
                }
            }
        }

        ship.FireCannon(p.Position);
    }

    void PlayerActionB(ref Player p)
    {
        //Check for a defect
        if (ship.BDefect[p.Position.PositionIndex] > 0)
        {
            AttackInternal(p.Position, InternalDamageType.B);
            return;
        }

        //Refill shield
        if (p.Position.IsTop())
        {
            if (ship.Reactors[p.Position.Zone] == 0 || ship.Shields[p.Position.Zone] == ship.ShieldsCap[p.Position.Zone]) // nothing happens regardless
                return;

            // we now know that we are transferring at least 1 energy

            BranchConditional(p.Position.Zone, Defects.shield);
            int deficit = ship.ShieldsCap[p.Position.Zone] - ship.Shields[p.Position.Zone];
            if (deficit == 0)
                return; // if it broke it might turn out that we no longer transfer energy

            BranchReactorFull(p.Position.Zone);

            //Pump over energy
            deficit = Math.Min(ship.Reactors[p.Position.Zone], deficit);
            ship.Shields[p.Position.Zone] += deficit;
            ship.Reactors[p.Position.Zone] -= deficit;
        }
        //Reactors
        else
        {
            //Main
            if (p.Position.IsMiddle())
            {
                ship.TryRefillMainReactor();
            }
            //Secondary
            else
            {
                if (ship.Reactors[1] == 0 || ship.Reactors[p.Position.Zone] == ship.ReactorsCap[p.Position.Zone]) // nothing happens regardless
                    return;

                // we now know that we are transferring at least 1 energy

                BranchConditional(p.Position.Zone, Defects.reactor);
                int deficit = ship.ReactorsCap[p.Position.Zone] - ship.Reactors[p.Position.Zone];
                if (deficit == 0)
                    return;

                BranchReactorFull(1);

                //Pump over energy
                deficit = Math.Min(ship.Reactors[1], deficit);
                ship.Reactors[p.Position.Zone] += deficit;
                ship.Reactors[1] -= deficit;
            }
        }
    }

    void PlayerActionC(ref Player p)
    {
        //Check for a defect
        if (ship.CDefect[p.Position.PositionIndex] > 0)
        {
            AttackInternal(p.Position, InternalDamageType.C);
            return;
        }

        switch (p.Position.PositionIndex)
        {
            //Interceptors
            case 0:
                //Check if requirements met
                if (p.AndroidState == AndroidState.Alive && ship.InterceptorReady)
                {
                    p.InIntercept = true;
                    ship.InterceptorReady = false;
                    p.MoveToSpace();
                    InterceptorDamage(ref p);
                }
                break;

            //Computer
            case 1:
                _didComputerThisPhase = true;
                break;

            //Androids
            case 2:
                if (p.AndroidState == AndroidState.None)
                {
                    p.AndroidState = ship.AndroidTopRight;
                    ship.AndroidTopRight = AndroidState.None;
                }
                else
                {
                    p.AndroidState = AndroidState.Alive;
                }
                break;

            //Androids
            case 3:
                if (p.AndroidState == AndroidState.None)
                {
                    p.AndroidState = ship.AndroidBottomLeft;
                    ship.AndroidBottomLeft = AndroidState.None;
                }
                else
                {
                    p.AndroidState = AndroidState.Alive;
                }
                break;

            //Observation
            case 4:
                _observationThisTurn++;
                break;

            //Fire rocket
            case 5:
                if (ship.CanFireRocket())
                    ship.FireRocket();
                break;
        }
    }

    void PlayerActionFight(ref Player p)
    {
        if (p.AndroidState == AndroidState.Alive)
        {
            InThreat thrt = AttackInternal(p.Position, InternalDamageType.Android);
            if (thrt != null)
            {
                if (thrt.fightBack)
                    p.AndroidState = AndroidState.Disabled;
            }
        }
    }

    void MoveThreats()
    {
        if (move_threats_iIn == inThreats.Count)
        {
            exThreats[move_threats_iEx].Move();
            move_threats_iEx++;
        }
        else if (move_threats_iEx == exThreats.Count)
        {
            inThreats[move_threats_iIn].Move();
            move_threats_iIn++;
        }
        else if (exThreats[move_threats_iEx].time < inThreats[move_threats_iIn].time)
        {
            exThreats[move_threats_iEx].Move();
            move_threats_iEx++;
        }
        else
        {
            inThreats[move_threats_iIn].Move();
            move_threats_iIn++;
        }
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
            score = score - 200 + turn;

        Scores.Add((int)score);

        return scoreMultiplier * score + scoreAddition;
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

    //Attack internal threat
    InThreat AttackInternal(Position position, InternalDamageType damageType)
    {
        //Find internal threat to attack
        for (int j = 0; j < inThreats.Count; j++)
        {
            if (!inThreats[j].beaten)
            {
                if (inThreats[j].DealDamage(position, damageType))
                    return inThreats[j];
            }
        }
        return null;
    }

    //Gets closest external threat
    int GetTargetEx(int zone, int range, ExDmgSource source)
    {
        int distance = 99;
        int target = -1;
        for (int i = 0; i < exThreats.Count; i++)
        {
            ExThreat et = exThreats[i];
            if (et.zone == zone)
            {
                int dist = et.GetDistance(range, source);
                if (dist < distance)
                {
                    target = i;
                    distance = dist;
                }
            }
        }

        return target;
    }

    //Deal damage with interceptors
    void InterceptorDamage(ref Player p)
    {
        //Attack internal threat
        if (ship.CDefect[6] > 0)
        {
            AttackInternal(Position.Space, InternalDamageType.C);
            return;
        }

        //Attack external threat
        int target = -1;
        for (int i = 0; i < exThreats.Count; i++)
        {
            if (exThreats[i].distanceRange == 1)
            {
                //Get first enemy
                if (target == -1)
                    target = i;
                //Deal damage to all in range
                else
                {
                    if (target >= 0)
                        exThreats[target].DealDamage(1, 1, ExDmgSource.intercept);
                    target = -2;
                    exThreats[i].DealDamage(1, 1, ExDmgSource.intercept);
                }
            }
        }
        //Deal damage to single target
        if (target >= 0)
            exThreats[target].DealDamage(3, 1, ExDmgSource.intercept);
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
        CreateProcessSteps,
        InternalTurnEnd,
        ExternalDamage,
        CleanThreats,
        CreateMoves,
        MoveThreat,
        SpawnThreat,
    }

    private readonly struct SimulationStep
    {
        public readonly SimulationStepType Type;
        public int PlayerIndex => _value1;
        public int ThreatIndex => _value1;
        public int CreatureId => _value1;
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

        public static SimulationStep NewCleanThreatsStep() => new(SimulationStepType.CleanThreats);

        public static SimulationStep NewCreateProcessStepsStep() => new(SimulationStepType.CreateProcessSteps);

        public static SimulationStep NewRocketUpdateStep() => new(SimulationStepType.RocketUpdate);

        public static SimulationStep NewPlayerActionStep(int playerIndex) => new(SimulationStepType.PlayerAction, value1: playerIndex);

        public static SimulationStep NewTurnStartStep() => new(SimulationStepType.TurnStart);

        public static SimulationStep NewThreatSpawnStep(int creatureId, int zone, bool isExternal)
            => new(SimulationStepType.SpawnThreat, value1:creatureId, value2: zone, bool1: isExternal);

        public static SimulationStep NewThreatSpawnStep(in Event @event) => NewThreatSpawnStep(@event.CreatureId, @event.Zone, @event.IsExternal);

        public static SimulationStep NewResetComputerStep() => new(SimulationStepType.ResetComputer);

        public static SimulationStep NewCheckComputerStep() => new(SimulationStepType.CheckComputer);

        public static SimulationStep NewObservationUpdateStep(bool startNewPhase) => new(SimulationStepType.ObservationUpdate, bool1: startNewPhase);
    }
}

public readonly record struct Event(bool IsExternal, int Turn, int Zone, int CreatureId);
