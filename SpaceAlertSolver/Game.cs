﻿using System.Collections.Immutable;
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


public class Game
{
    public static List<int> Scores = new();
    private static readonly int[] _obsBonus = new int[] { 0, 1, 2, 3, 5, 7, 9, 11, 13, 15, 17 };
    public const int NUM_PHASES = 3;

    internal readonly Ship ship;
    public Player[] players = null!;
    public ImmutableArray<Trajectory> trajectories;
    public ImmutableArray<Event> events;
    public readonly List<ExThreat> exThreats = new();
    public readonly List<InThreat> inThreats = new();
    int phase;
    double score;
    readonly bool[] phaseComputer = new bool[NUM_PHASES];
    bool gameover;
    int eventIdx;

    int exSlain, exSurvived, inSlain, inSurvived;

    readonly int[] observation = new int[NUM_PHASES];
    int observationCount;

    double scoreMultiplier;
    double scoreAddition;

    public int ScoutBonus;

    // simulation phase
    SimulationPhase sp;
    int turn;
    int actions_player_i;
    int internal_turn_end_i;
    int external_damage_i;
    int move_threats_iEx;
    int move_threats_iIn;
    Act action;

    public Game()
    {
        ship = new(this);
    }

    public void Init(Game other)
    {
        ship.Init(other.ship);
        InitPlayers(other.players);
        trajectories = other.trajectories;
        events = other.events;
        exThreats.Clear();
        exThreats.AddRange(other.exThreats.Select(t => t.Clone(this)));
        inThreats.Clear();
        inThreats.AddRange(other.inThreats.Select(t => t.Clone(this)));

        phase = other.phase;
        score = other.score;
        other.phaseComputer.CopyTo(phaseComputer, 0);
        gameover = other.gameover;
        eventIdx = other.eventIdx;
        exSlain = other.exSlain;
        exSurvived = other.exSurvived;
        inSlain = other.inSlain;
        inSurvived = other.inSurvived;

        other.observation.CopyTo(observation, 0);
        observationCount = other.observationCount;

        scoreMultiplier = 1.0;
        scoreAddition = 0.0;

        ScoutBonus = other.ScoutBonus;

        sp = other.sp;
        turn = other.turn;
        actions_player_i = other.actions_player_i;
        internal_turn_end_i = other.internal_turn_end_i;
        external_damage_i = other.external_damage_i;
        move_threats_iEx = other.move_threats_iEx;
        move_threats_iIn = other.move_threats_iIn;
        action = other.action;
    }

    public void Init(Player[] players, ImmutableArray<Trajectory> trajectories, ImmutableArray<Event> events)
    {
        ship.Init();
        InitPlayers(players);
        this.trajectories = trajectories;
        this.events = events;
        exThreats.Clear();
        inThreats.Clear();
        phase = 0;
        score = 0.0;
        Array.Fill(phaseComputer, false);
        gameover = false;
        eventIdx = 0;
        exSlain = 0;
        inSlain = 0;
        exSurvived = 0;
        inSurvived = 0;
        Array.Fill(observation, 0);
        observationCount = 0;
        scoreMultiplier = 1.0;
        scoreAddition = 0.0;
        ScoutBonus = 0;
        sp = SimulationPhase.TurnPrep;
        turn = 1;
        actions_player_i = 0;
        internal_turn_end_i = 0;
        external_damage_i = 0;
        move_threats_iEx = 0;
        move_threats_iIn = 0;
        action = Act.Empty;
    }

    private void InitPlayers(Player[] other)
    {
        if (players == null || players.Length != other.Length)
            players = new Player[other.Length];

        other.CopyTo(players, 0);
    }

    internal ref Player GetCurrentTurnPlayer()
    {
        return ref players[actions_player_i];
    }

    //Simulate the game
    public double Simulate()
    {
        while (true)
        {
            switch (sp)
            {
                case SimulationPhase.TurnPrep:
                    if (turn > 12 || gameover)
                    {
                        RocketFire();
                        sp = SimulationPhase.FinalExternalDamage;
                        external_damage_i = 0;
                        break;
                    }
                    OnTurnStart();
                    sp = SimulationPhase.ReadAction;
                    actions_player_i = 0;
                    break;

                case SimulationPhase.ReadAction:
                    if (actions_player_i >= players.Length)
                    {
                        sp = SimulationPhase.RocketFire;
                        break;
                    }
                    action = players[actions_player_i].GetNextAction();
                    sp = SimulationPhase.PerformAction;
                    break;

                case SimulationPhase.PerformAction:
                    PlayerActions(ref players[actions_player_i]);
                    sp = SimulationPhase.ReadAction;
                    actions_player_i++;
                    break;

                case SimulationPhase.RocketFire:
                    RocketFire();
                    sp = SimulationPhase.InternalTurnEnd;
                    internal_turn_end_i = 0;
                    break;

                case SimulationPhase.InternalTurnEnd:
                    if (internal_turn_end_i == inThreats.Count)
                    {
                        sp = SimulationPhase.CleanupInternal;
                        break;
                    }
                    InternalTurnEnd();
                    internal_turn_end_i++;
                    break;

                case SimulationPhase.CleanupInternal:
                    CleanupInternal();
                    sp = SimulationPhase.ExternalDamage;
                    external_damage_i = 0;
                    break;

                case SimulationPhase.ExternalDamage:
                    if (external_damage_i == exThreats.Count)
                    {
                        sp = SimulationPhase.CleanupExternal;
                        break;
                    }
                    ExternalDamage();
                    external_damage_i++;
                    break;

                case SimulationPhase.FinalExternalDamage:
                    if (external_damage_i == exThreats.Count)
                    {
                        sp = SimulationPhase.FinalCleanup;
                        break;
                    }
                    ExternalDamage();
                    external_damage_i++;
                    break;

                case SimulationPhase.CleanupExternal:
                    CleanupExternal();
                    sp = SimulationPhase.MoveThreats;
                    move_threats_iEx = 0;
                    move_threats_iIn = 0;
                    break;

                case SimulationPhase.FinalCleanup:
                    CleanupInternal();
                    CleanupExternal();
                    sp = SimulationPhase.FinalMoveThreats;
                    move_threats_iEx = 0;
                    move_threats_iIn = 0;
                    break;

                case SimulationPhase.MoveThreats:
                    if (move_threats_iEx == exThreats.Count
                        && move_threats_iIn == inThreats.Count)
                    {
                        sp = SimulationPhase.TurnCleanup;
                        break;
                    }
                    MoveThreats(); // no increment it will happen in the function
                    break;

                case SimulationPhase.FinalMoveThreats:
                    if (move_threats_iEx == exThreats.Count
                        && move_threats_iIn == inThreats.Count)
                    {
                        OnTurnEnd();
                        sp = SimulationPhase.FinalScoring;
                        break;
                    }
                    MoveThreats(); // no increment it will happen in the function
                    break;

                case SimulationPhase.TurnCleanup:
                    OnTurnEnd();
                    sp = SimulationPhase.TurnPrep;
                    turn++;
                    break;

                case SimulationPhase.FinalScoring:
                    CleanupInternal();
                    CleanupExternal();
                    return CalculateScore();
            }
        }
    }

    void OnTurnStart()
    {
        //Set phase
        if (turn <= 3)
            phase = 0;
        else if (turn <= 7)
            phase = 1;
        else
            phase = 2;

        //Reset variables
        observationCount = 0;
        ship.OnTurnStart();

        //Check computer
        if (turn == 3 || turn == 6 || turn == 10)
        {
            //Delay actions if computer failed
            if (!phaseComputer[phase])
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].Position.IsInShip())
                        players[i].DelayNext();
                }
            }
        }

        //Check event
        if (SpawnThreat(turn, eventIdx))
            eventIdx++;
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
                phaseComputer[phase] = true;
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
                observationCount++;
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

    void RocketFire()
    {
        if (!ship.RocketReady)
            return;

        int distance = int.MaxValue;
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
        //Deal damage
        target?.DealDamage(3, 2, ExDmgSource.rocket);
    }

    void InternalTurnEnd()
    {
        inThreats[internal_turn_end_i].ProcessTurnEnd();
    }

    void CleanupInternal()
    {
        for (int i = inThreats.Count - 1; i >= 0; i--)
        {
            //Threat is gone
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
    }

    void ExternalDamage()
    {
        exThreats[external_damage_i].ProcessDamage();
    }

    void CleanupExternal()
    {
        for (int i = exThreats.Count - 1; i >= 0; i--)
        {
            //Threat is gone
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

    void OnTurnEnd()
    {
        ship.OnTurnEnd();

        //Calculate observation bonus
        observation[phase] = Math.Max(observation[phase], _obsBonus[observationCount]);

        //Check if gameover
        for (int i = 0; i < 3; i++)
        {
            if (ship.Damage[i] >= 7)
                gameover = true;
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
            score += observation[0] + observation[1] + observation[2];

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

    //Spawn a threat
    bool SpawnThreat(int turn, int eventIdx)
    {
        if (eventIdx < events.Length && events[eventIdx].Turn == turn)
        {
            //Summon threat
            Event ev = events[eventIdx];
            if (ev.IsExternal)
            {
                exThreats.Add(ThreatFactory.SummonEx(ev.CreatureId, trajectories[ev.Zone], ev.Zone, this, turn));
            }
            else
            {
                inThreats.Add(ThreatFactory.SummonIn(ev.CreatureId, trajectories[3], this, turn));
            }
            return true;
        }
        return false;
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

    private enum SimulationPhase
    {
        TurnPrep,
        ReadAction,
        PerformAction,
        RocketFire,
        InternalTurnEnd,
        CleanupInternal,
        ExternalDamage,
        CleanupExternal,
        MoveThreats,
        TurnCleanup,
        FinalExternalDamage,
        FinalCleanup,
        FinalMoveThreats,
        FinalScoring
    }
}

public readonly record struct Event(bool IsExternal, int Turn, int Zone, int CreatureId);
