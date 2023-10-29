using System.Diagnostics;
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
    public static List<int> scores = new List<int>();

    Ship ship;
    Player[] players;
    Trajectory[] trajectories;
    Event[] events;
    public List<ExThreat> exThreats;
    public List<InThreat> inThreats;
    int phase;
    double score;
    bool[] phaseComputer;
    bool gameover;
    int eventIdx;

    int exSlain, exSurvived, inSlain, inSurvived;

    int[] observation;
    int observationCount;
    static int[] obsBonus = new int[] { 0, 1, 2, 3, 5, 7, 9, 11, 13, 15, 17 };

    double scoreMultiplier = 1.0;
    double scoreAddition = 0.0;

    // simulation phase
    SimulationPhase sp = SimulationPhase.SimulationPrep;
    int turn;
    int actions_player_i;
    int internal_turn_end_i;
    int external_damage_i;
    int move_threats_iEx;
    int move_threats_iIn;

    public Game(Game other)
    {
        players = new Player[other.players.Length];
        ship = new Ship(other.ship, this, players);
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player(other.players[i], other.ship.androids, ship.androids);
        }
        trajectories = other.trajectories;
        events = other.events;

        exThreats = new List<ExThreat>();
        for (int i = 0; i < other.exThreats.Count; i++)
        {
            exThreats.Add(other.exThreats[i].Clone(ship));
        }
        inThreats = new List<InThreat>();
        for (int i = 0; i < other.inThreats.Count; i++)
        {
            inThreats.Add(other.inThreats[i].Clone(ship));
        }

        phase = other.phase;
        score = other.score;
        phaseComputer = Extension.CopyArray(other.phaseComputer);
        gameover = other.gameover;
        eventIdx = other.eventIdx;
        exSlain = other.exSlain;
        exSurvived = other.exSurvived;
        inSlain = other.inSlain;
        inSurvived = other.inSurvived;
        observation = Extension.CopyArray(other.observation);
        observationCount = other.observationCount;

        sp = other.sp;
        turn = other.turn;
        actions_player_i = other.actions_player_i;
        internal_turn_end_i = other.internal_turn_end_i;
        external_damage_i = other.external_damage_i;
        move_threats_iEx = other.move_threats_iEx;
        move_threats_iIn = other.move_threats_iIn;
    }

    public Game(Player[] players, Trajectory[] trajectories, Event[] events)
    {
        this.players = new Player[players.Length];
        for (int i = 0; i < players.Length; i++)
            this.players[i] = players[i].Copy();

        this.trajectories = trajectories;
        this.events = events;
    }

    //Simulate the game
    public double Simulate()
    {
        while (true)
        {
            switch (sp)
            {
                case SimulationPhase.SimulationPrep:
                    SimulationPrep();
                    sp = SimulationPhase.TurnPrep;
                    turn = 1;
                    break;

                case SimulationPhase.TurnPrep:
                    if (turn > 12 || gameover)
                    {
                        RocketFire();
                        sp = SimulationPhase.FinalExternalDamage;
                        external_damage_i = 0;
                        break;
                    }
                    TurnPrep();
                    sp = SimulationPhase.Actions;
                    actions_player_i = 0;
                    break;

                case SimulationPhase.Actions:
                    if (actions_player_i == players.Length)
                    {
                        sp = SimulationPhase.RocketFire;
                        break;
                    }
                    PlayerActions();
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
                        sp = SimulationPhase.FinalScoring;
                        break;
                    }
                    MoveThreats(); // no increment it will happen in the function
                    break;

                case SimulationPhase.TurnCleanup:
                    TurnCleanup();
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

    void SimulationPrep()
    {
        gameover = false;
        eventIdx = 0;
        ship = new Ship(this, players);
        exThreats = new List<ExThreat>();
        inThreats = new List<InThreat>();

        observation = new int[3];
        phaseComputer = new bool[3];
    }

    void TurnPrep()
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
        ship.cannonFired = 0;
        ship.liftUsed = ship.liftReset;

        //Check computer
        if (turn == 3 || turn == 6 || turn == 10)
        {
            //Delay actions if computer failed
            if (!phaseComputer[phase])
            {
                foreach (Player p in players)
                {
                    if (p.position < 6)
                        p.Delay(turn - 1);
                }
            }
        }

        //Check event
        if (SpawnThreat(turn, eventIdx))
            eventIdx++;
    }

    void PlayerActions()
    {
        int t = turn - 1;

        Player p = players[actions_player_i];
        p.lastAction = t;
        Act a = p.actions[t];

        //Exit if dead
        if (!p.alive)
            return;

        //Check interceptor controls
        if (p.inIntercept)
        {
            if (a == Act.fight)
            {
                //Keep fighting
                InterceptorDamage();
                return;
            }
            else
            {
                //Return
                p.inIntercept = false;
                ship.interceptorReady = true;
                p.Move(0);
                ApplyStatusEffect(p, t);
                p.Delay(t);
                return;
            }
        }

        //Perform action
        switch (a)
        {
            case Act.left:
                PlayerActionMove(p, t, true);
                break;
            case Act.right:
                PlayerActionMove(p, t, false);
                break;
            case Act.lift:
                PlayerActionLift(p);
                break;
            case Act.A:
                PlayerActionA(p);
                break;
            case Act.B:
                PlayerActionB(p);
                break;
            case Act.C:
                PlayerActionC(p);
                break;
            //Empty for now
            case Act.fight:
                PlayerActionFight(p);
                break;
            case Act.empty:
                break;
        }
    }

    void PlayerActionMove(Player p, int t, bool left)
    {
        Debug.Assert(true);

        if (left)
        {
            if (p.position != 0 && p.position != 3)
                p.Move(p.position - 1);
        }
        else
        {
            if (p.position != 2 && p.position != 5)
                p.Move(p.position + 1);
        }
        ApplyStatusEffect(p, t);
    }

    void PlayerActionLift(Player p)
    {
        Debug.Assert(true);

        int t = turn - 1;
        int z = p.position % 3;

        // branching
        if (t < 11 && p.actions[t + 1] != Act.empty) // no branching needed if nothing to be delayed
        {
            BranchConditional(z, Defects.lift);
        }

        //Check if elevator was used
        int bm = 1 << z;
        if ((ship.liftUsed & bm) == bm)
            p.Delay(t + 1);
        ship.liftUsed |= bm;
        //Move
        if (p.position < 3)
            p.Move(p.position + 3);
        else
            p.Move(p.position - 3);
        ApplyStatusEffect(p, t);
    }

    void PlayerActionA(Player p)
    {
        Debug.Assert(true);

        int z = p.position % 3;

        //Check if can fire
        int bm = 1 << p.position;
        if ((ship.cannonFired & bm) == bm)
            return;

        if (p.position < 3)
        {
            //Main guns
            //Drain energy
            if (ship.reactors[z] > 0)
            {
                BranchReactorFull(z);

                //Find target
                int target = GetTargetEx(z, 3, ExDmgSource.laser);

                if (target != -1)
                {
                    BranchConditional(z, Defects.weapontop);

                    exThreats[target].DealDamage(ship.laserDamage[z], 3, ExDmgSource.laser);
                }

                ship.reactors[z]--;
            }
        }
        else
        {
            //Secondary guns
            //Impulse cannon
            if (z == 1)
            {
                //Drain energy
                if (ship.reactors[z] > 0)
                {
                    BranchReactorFull(z);
                    BranchConditional(z, Defects.weaponbot);

                    ship.reactors[z]--;
                    foreach (ExThreat et in exThreats)
                        et.DealDamage(1, ship.plasmaDamage[z], ExDmgSource.impulse);
                }
            }
            //Plasma cannon
            else
            {
                //Find target
                int target = GetTargetEx(z, 3, ExDmgSource.plasma);
                //Deal damage
                if (target != -1)
                {
                    BranchConditional(z, Defects.weaponbot);
                    exThreats[target].DealDamage(ship.plasmaDamage[z], 3, ExDmgSource.laser);
                }
            }
        }

        ship.cannonFired |= bm;
    }

    void PlayerActionB(Player p)
    {
        Debug.Assert(true);

        int z = p.position % 3;

        //Check for a defect
        if (ship.BDefect[p.position] > 0)
        {
            AttackInternal(p.position, InDmgSource.B);
            return;
        }

        //Refill shield
        if (p.position < 3)
        {
            if (ship.reactors[z] == 0 || ship.shields[z] == ship.shieldsCap[z]) // nothing happens regardless
                return;

            // we now know that we are transferring at least 1 energy

            BranchConditional(z, Defects.shield);
            int deficit = ship.shieldsCap[z] - ship.shields[z];
            if (deficit == 0)
                return; // if it broke it might turn out that we no longer transfer energy

            BranchReactorFull(z);

            //Pump over energy
            deficit = Math.Min(ship.reactors[z], deficit);
            ship.shields[z] += deficit;
            ship.reactors[z] -= deficit;
        }
        //Reactors
        else
        {
            //Main
            if (z == 1)
            {
                //Fill main
                if (ship.capsules == 0)
                    return;

                // no need to branch! it's at the cap so we can branch at a later stage
                ship.reactors[z] = ship.reactorsCap[z];
                ship.capsules--;
            }
            //Secondary
            else
            {
                if (ship.reactors[1] == 0 || ship.reactors[z] == ship.reactorsCap[z]) // nothing happens regardless
                    return;

                // we now know that we are transferring at least 1 energy

                BranchConditional(z, Defects.reactor);
                int deficit = ship.reactorsCap[z] - ship.reactors[z];
                if (deficit == 0)
                    return;

                BranchReactorFull(1);

                //Pump over energy
                deficit = Math.Min(ship.reactors[1], deficit);
                ship.reactors[z] += deficit;
                ship.reactors[1] -= deficit;
            }
        }
    }

    void PlayerActionC(Player p)
    {
        //Check for a defect
        if (ship.CDefect[p.position] > 0)
        {
            AttackInternal(p.position, InDmgSource.C);
            return;
        }

        switch (p.position)
        {
            //Interceptors
            case 0:
                //Check if requirements met
                if (p.team != null && p.team.alive && ship.interceptorReady)
                {
                    p.inIntercept = true;
                    ship.interceptorReady = false;
                    p.Move(6);
                    InterceptorDamage();
                }
                break;

            //Computer
            case 1:
                phaseComputer[phase] = true;
                break;

            //Androids
            case 2:
                //Take androids
                if (!ship.androids[0].active)
                {
                    ship.androids[0].active = true;
                    p.team = ship.androids[0];
                }
                //Repair androids
                else if (p.team != null)
                    p.team.alive = true;
                break;

            //Androids
            case 3:
                //Take androids
                if (!ship.androids[1].active)
                {
                    ship.androids[1].active = true;
                    p.team = ship.androids[1];
                }
                else if (p.team != null)
                    p.team.alive = true;
                break;

            //Observation
            case 4:
                observationCount++;
                break;

            //Fire rocket
            case 5:
                if (ship.rockets > 0)
                    ship.rocketFired = true;
                ship.rockets--;
                break;
        }
    }

    void PlayerActionFight(Player p)
    {
        if (p.team != null && p.team.alive)
        {
            InThreat thrt = AttackInternal(p.position, InDmgSource.android);
            if (thrt != null)
            {
                if (thrt.fightBack)
                    p.team.alive = false;
            }
        }
    }

    void RocketFire()
    {
        if (ship.rocketReady)
        {
            ship.rocketReady = false;
            int distance = 99;
            int target = -1;
            for (int i = 0; i < exThreats.Count; i++)
            {
                ExThreat et = exThreats[i];
                int dist = et.GetDistance(2, ExDmgSource.rocket);
                if (dist < distance)
                {
                    target = i;
                    distance = dist;
                }
            }
            //Deal damage
            if (target != -1)
                exThreats[target].DealDamage(3, 2, ExDmgSource.rocket);
        }
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

    void TurnCleanup()
    {
        //Move rocket
        if (ship.rocketFired)
        {
            ship.rocketReady = true;
            ship.rocketFired = false;
        }

        //Calculate observation bonus
        observation[phase] = Math.Max(observation[phase], obsBonus[observationCount]);

        //Check if gameover
        for (int i = 0; i < 3; i++)
        {
            if (ship.damage[i] >= 7)
                gameover = true;
        }
    }

    double CalculateScore()
    {
        //Count damage
        int highestDamage = 0;
        for (int i = 0; i < 3; i++)
        {
            score -= ship.damage[i];
            highestDamage = Math.Max(ship.damage[i], highestDamage);
        }
        score -= highestDamage;

        //Count dead crew
        foreach (Player p in players)
        {
            if (!p.alive)
                score -= 2;
        }
        //Count dead androids
        if (!ship.androids[0].alive)
            score--;
        if (!ship.androids[1].alive)
            score--;

        //Observation bonus
        if (!gameover)
            score += observation[0] + observation[1] + observation[2];

        //Gameover penalty
        if (gameover)
            score = score - 200 + turn;

        scores.Add((int)score);

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
            && ship.defectStates[zone][(int)defect] == DefectState.undetermined);

        double chanceOfBreaking = ship.ChanceOfDefect(zone);
        if (chanceOfBreaking < 1.0)
        {
            Game branch = new Game(this);
            branch.ship.NotDefect(zone, defect);

            scoreAddition += branch.Simulate() * (1.0 - chanceOfBreaking) * scoreMultiplier;
            scoreMultiplier *= chanceOfBreaking;
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

        if (ship.defectStates[zone][(int)defect] == DefectState.undetermined)
        {
            Branch(zone, defect);
        }
    }

    public void BranchReactorFull(int zone)
    {
        Debug.Assert(zone >= 0 && zone < 3);

        if (ship.reactors[zone] == ship.reactorsCap[zone])
        {
            BranchConditional(zone, Defects.reactor);
        }
    }

    public void BranchShieldFull(int zone)
    {
        Debug.Assert(zone >= 0 && zone < 3);

        if (ship.shields[zone] == ship.shieldsCap[zone])
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
                exThreats.Add(ThreatFactory.SummonEx(ev.CreatureId, trajectories[ev.Zone], ev.Zone, ship, turn));
            }
            else
            {
                inThreats.Add(ThreatFactory.SummonIn(ev.CreatureId, trajectories[3], ship, turn));
            }
            return true;
        }
        return false;
    }

    //Attack internal threat
    InThreat AttackInternal(int position, InDmgSource source)
    {
        //Find internal threat to attack
        for (int j = 0; j < inThreats.Count; j++)
        {
            if (!inThreats[j].beaten)
            {
                if (inThreats[j].DealDamage(position, source))
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
    void InterceptorDamage()
    {
        //Attack internal threat
        if (ship.CDefect[6] > 0)
        {
            AttackInternal(6, InDmgSource.C);
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

    void ApplyStatusEffect(Player p, int turn)
    {
        //Check status effect
        if (ship.stationStatus[p.position] != 0)
        {
            // Delay
            if ((ship.stationStatus[p.position] & 1) == 1)
                p.Delay(turn + 1);
            // Kill
            else if ((ship.stationStatus[p.position] & 2) == 2)
                p.Kill();
        }
    }

    public string GetDebug()
    {
        StringBuilder output = new StringBuilder();
        output.AppendFormat("DMG: {0} {1} {2}\n", ship.damage[0], ship.damage[1], ship.damage[2]);
        output.AppendFormat("OBS: {0} {1} {2}\n", observation[0], observation[1], observation[2]);
        output.AppendFormat("P Pos: {0} {1} {2} {3} {4}\n", players[0].position, players[1].position, players[2].position, players[3].position, players[4].position);
        output.AppendFormat("LastAct: {0} {1} {2} {3} {4}\n", ActToString(players[0].actions[11]), ActToString(players[1].actions[11]), ActToString(players[2].actions[11]), ActToString(players[3].actions[11]), ActToString(players[4].actions[11]));
        output.AppendFormat("Alive: {0} {1} {2} {3} {4}\n", players[0].alive, players[1].alive, players[2].alive, players[3].alive, players[4].alive);
        output.AppendFormat("ExKill: {0} | ExSurv: {1} | InKill: {2} | InSurv: {3}\n", exSlain, exSurvived, inSlain, inSurvived);
        output.AppendFormat("Reactors: {0} {1} {2}\n", ship.reactors[0], ship.reactors[1], ship.reactors[2]);
        output.AppendFormat("Shields: {0} {1} {2}\n", ship.shields[0], ship.shields[1], ship.shields[2]);
        output.AppendFormat("Caps: {0} | Rockets: {1}", ship.capsules, ship.rockets);

        return output.ToString();
    }

    string ActToString(Act a)
    {
        switch (a)
        {
            case Act.A:
                return "A";
            case Act.B:
                return "B";
            case Act.C:
                return "C";
            case Act.empty:
                return "-";
            case Act.fight:
                return "Robot";
            case Act.left:
                return "Red";
            case Act.right:
                return "Blue";
            case Act.lift:
                return "Lift";
            default:
                return "";
        }
    }
}

public enum SimulationPhase
{
    SimulationPrep,
    TurnPrep,
    Actions,
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

public enum Act
{
    left,
    right,
    lift,
    A,
    B,
    C,
    fight,
    empty,
    tl,
    tm,
    tr,
    dl,
    dm,
    dr,
    hA,
    hB,
    hFight
}

//Event
public readonly struct Event
{
    public readonly bool IsExternal;
    public readonly int Turn, Zone, CreatureId;

    public Event(bool isExternal, int turn, int zone, int creatureId)
    {
        IsExternal = isExternal;
        Turn = turn;
        Zone = zone;
        CreatureId = creatureId;
    }
}
