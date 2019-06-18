﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    /* ----------------
     *Adjusted rulings: 
     * -Ship damage doesnt cause defects
     * -No defects
     * 
     * Questionable stuff:
     * Surviving asteroids doesnt cause asteroid destruction effect
     * Scout is immune to laser instead of not being able to get targeted
    */


    public class Game
    {
        Ship ship;
        Player[] players;
        Trajectory[] trajectories;
        Event[] events;
        public List<ExThreat> exThreats;
        public List<InThreat> inThreats;
        int turn;
        int phase;
        int score;
        bool[] phaseComputer;

        int exSlain, exSurvived, inSlain, inSurvived;

        int[] observation;
        int observationCount;
        int[] obsBonus = new int[] { 0, 1, 2, 3, 5, 7, 9, 11, 13, 15, 17 };

        public void Setup(Player[] players, Trajectory[] trajectories, Event[] events)
        {
            this.players = new Player[players.Length];
            for (int i = 0; i < players.Length; i++)
                this.players[i] = players[i].Copy();

            this.trajectories = trajectories;
            this.events = events;
        }

        //Simulate the game
        public int Simulate()
        {
            bool gameover = false;
            int pCount = players.Length;
            int eventIdx = 0;
            turn = 1;
            ship = new Ship(players, this);
            exThreats = new List<ExThreat>();
            inThreats = new List<InThreat>();

            observation = new int[3];
            phaseComputer = new bool[3];

            //Gameloop
            while (turn <= 12)
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
                for (int i = 0; i < 6; i++)
                    ship.cannonFired[i] = false;
                for (int i = 0; i < 3; i++)
                    ship.liftUsed[i] = false;

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
                if (SpawnThreat(eventIdx))
                    eventIdx++;

                //Perform player actions
                PlayerActions(turn - 1);

                //Process internal damage and cleanup
                ResolveInDamage();

                //Rocket hits
                RocketFire();

                //Apply damage to threats and check if dead
                ResolveExDamage();

                //Move threats (Fix sequencing for internal threats)
                MoveThreats();

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
                if (gameover)
                    break;

                turn++;
            }

            //Final movement if not gameover
            if (!gameover)
            {
                //Fire last rocket
                RocketFire();
                //Check damage
                ResolveExDamage();
                //Move
                MoveThreats();
                //Cleanup dead
                ResolveInDamage();
                ResolveExDamage();
            }

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

            return score;
        }

        //Moves threats in order
        void MoveThreats()
        {
            int iEx = 0, iIn = 0;
            while (iEx < exThreats.Count || iIn < inThreats.Count)
            {
                if(iIn == inThreats.Count)
                {
                    exThreats[iEx].Move();
                    iEx++;
                }
                else if(iEx == exThreats.Count)
                {
                    inThreats[iIn].Move();
                    iIn++;
                }
                else if(exThreats[iEx]. time < inThreats[iIn].time)
                {
                    exThreats[iEx].Move();
                    iEx++;
                }
                else
                {
                    inThreats[iIn].Move();
                    iIn++;
                }
            }
        }

        //Resolve damage against internal threats
        void ResolveInDamage()
        {
            for (int i = 0; i < inThreats.Count; i++)
                inThreats[i].ProcessTurnEnd();
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

        //Resolve damage against external threats
        void ResolveExDamage()
        {
            //Process damage
            for (int i = 0; i < exThreats.Count; i++)
                exThreats[i].ProcessDamage();

            //Cleanup dead
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

        //Set rocket target and damage
        void RocketFire()
        {
            if (ship.rocketReady)
            {
                ship.rocketReady = false;
                int target = -1;
                int distance = int.MaxValue;
                for (int i = 0; i < exThreats.Count; i++)
                {
                    //Moloch edgecase
                    if(exThreats[i] is Moloch)
                    {
                        exThreats[i].DealDamage(3, 2, ExDmgSource.rocket);
                        target = -1;
                        break;
                    }
                    //Check if valid target
                    if (!exThreats[i].rocketImmune && exThreats[i].distanceRange <= 2 && exThreats[i].distance < distance)
                    {
                        target = i;
                        distance = exThreats[i].distance;
                    }
                }
                //Deal damage
                if (target != -1)
                    exThreats[target].DealDamage(3, 2, ExDmgSource.rocket);
            }
        }

        //Spawn a threat
        bool SpawnThreat(int eventIdx)
        {
            if (eventIdx < events.Length && events[eventIdx].turn == turn)
            {
                //Summon threat
                Event ev = events[eventIdx];
                if (ev.external)
                    exThreats.Add(ThreatFactory.SummonEx(ev.creature, trajectories[ev.zone], ev.zone, ship, turn));
                else
                    inThreats.Add(ThreatFactory.SummonIn(ev.creature, trajectories[3], ship, turn));
                return true;
            }
            return false;
        }

        //All players perform actions
        void PlayerActions(int t)
        {
            //Loop over players
            for (int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                p.lastAction = t;
                Act a = p.actions[t];
                int z = p.position % 3;

                //Exit if dead
                if (!p.alive)
                    continue;

                //Check interceptor controls
                if (p.inIntercept)
                {
                    if(a == Act.fight)
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
                        p.Delay(t);
                        return;
                    }
                }

                //Perform action
                switch (a)
                {
                    //Movement
                    case Act.left:
                        if (p.position != 0 && p.position != 3)
                            p.Move(p.position - 1);
                        break;
                    case Act.right:
                        if (p.position != 2 && p.position != 5)
                            p.Move(p.position + 1);
                        break;
                    case Act.lift:
                        //Check if elevator was used
                        if (ship.liftUsed[z])
                            p.Delay(t + 1);
                        ship.liftUsed[z] = true;
                        //Move
                        if (p.position < 3)
                            p.Move(p.position + 3);
                        else
                            p.Move(p.position - 3);
                        break;

                    //Actions
                    case Act.A:
                        //Check if can fire
                        if (ship.cannonFired[p.position])
                            break;
                        ship.cannonFired[p.position] = true;

                        if (p.position < 3)
                        {
                            //Main guns
                            //Drain energy
                            if (ship.reactors[z] == 0)
                                break;
                            ship.reactors[z]--;
                            //Find target
                            int target = -1;
                            int distance = int.MaxValue;
                            for (int j = 0; j < exThreats.Count; j++)
                            {
                                ExThreat et = exThreats[j];
                                if (et.zone == z && et.distance < distance)
                                {
                                    target = j;
                                    distance = et.distance;
                                }
                            }
                            //Deal damage
                            if (target != -1)
                                exThreats[target].DealDamage(ship.laserDamage[z], 3, ExDmgSource.laser);
                        }
                        else
                        {
                            //Secondary guns
                            //Impulse cannon
                            if (z == 1)
                            {
                                //Drain energy
                                if (ship.reactors[z] == 0)
                                    break;
                                ship.reactors[z]--;
                                //Hit all enemies
                                foreach (ExThreat et in exThreats)
                                    et.DealDamage(1, ship.pulseRange, ExDmgSource.impulse);
                            }
                            //Plasma cannon
                            else
                            {
                                //Find target
                                int target = -1;
                                int distance = int.MaxValue;
                                for (int j = 0; j < exThreats.Count; j++)
                                {
                                    ExThreat et = exThreats[j];
                                    if (et.zone == z && et.distance < distance)
                                    {
                                        target = j;
                                        distance = et.distance;
                                    }
                                }
                                //Deal damage
                                if (target != -1)
                                    exThreats[target].DealDamage(ship.plasmaDamage[z], 3, ExDmgSource.plasma);
                            }
                        }
                        break;

                    case Act.B:
                        //Refill shield
                        if (p.position < 3)
                        {
                            int deficit = ship.shieldsCap[z] - ship.shields[z];
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
                                if (ship.capsules > 0)
                                {
                                    ship.reactors[z] = ship.reactorsCap[z];
                                    ship.capsules--;
                                }
                            }
                            //Secondary
                            else
                            {
                                //Pump over energy
                                int deficit = ship.reactorsCap[z] - ship.reactors[z];
                                deficit = Math.Min(ship.reactors[1], deficit);
                                ship.reactors[z] += deficit;
                                ship.reactors[1] -= deficit;
                            }
                        }
                        break;
                    case Act.C:
                        switch (p.position)
                        {
                            //Interceptors
                            case 0:
                                //Check if requirements met
                                if(p.team != null && p.team.alive && ship.interceptorReady)
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
                        break;

                    //Empty for now
                    case Act.fight:
                        if(p.team != null && p.team.alive)
                        {
                            InThreat thrt = AttackInternal(p.position, InDmgSource.android);
                            if(thrt != null)
                            {
                                if (thrt.fightBack)
                                    p.team.alive = false;
                            }
                        }
                        break;
                    case Act.empty:
                        break;
                }
            }
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

        //Deal damage with interceptors
        void InterceptorDamage()
        {
            int target = -1;
            for(int i = 0; i < exThreats.Count; i++)
            {
                if(exThreats[i].distanceRange == 1)
                {
                    //Get first enemy
                    if (target == -1)
                        target = i;
                    //Deal damage to all in range
                    else
                    {
                        if(target >= 0)
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

        public string GetDebug()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("DMG: {0} {1} {2}\n", ship.damage[0], ship.damage[1], ship.damage[2]);
            output.AppendFormat("OBS: {0} {1} {2}\n", observation[0], observation[1], observation[2]);
            output.AppendFormat("P Pos: {0} {1} {2} {3} {4}\n", players[0].position, players[1].position, players[2].position, players[3].position, players[4].position);
            output.AppendFormat("LastAct: {0} {1} {2} {3} {4}\n", players[0].actions[11], players[1].actions[11], players[2].actions[11], players[3].actions[11], players[4].actions[11]);
            output.AppendFormat("ExKill: {0} | ExSurv: {1} | InKill: {2} | InSurv: {3}\n", exSlain, exSurvived, inSlain, inSurvived);
            output.AppendFormat("Reactors: {0} {1} {2}\n", ship.reactors[0], ship.reactors[1], ship.reactors[2]);
            output.AppendFormat("Shields: {0} {1} {2}\n", ship.shields[0], ship.shields[1], ship.shields[2]);
            output.AppendFormat("Caps: {0} | Rockets: {1}", ship.capsules, ship.rockets);

            return output.ToString();
        }
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
        empty
    }

    //Event
    public class Event
    {
        public bool external;
        public int turn, zone, creature;

        public Event(bool ex, int turn, int zone, int creature)
        {
            this.external = ex;
            this.turn = turn;
            this.zone = zone;
            this.creature = creature;
        }
    }
}
