using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    /* ----------------
     *Adjusted rulings: 
     * -No combat androids
     * -No interceptors
     * -Ship damage doesnt cause defects
     * -No final move phase
     * -No internal threats
     * 
     * Questionable stuff:
     * Surviving asteroids doesnt cause asteroid destruction effect
    */


    class Game
    {
        Ship ship;
        Player[] players;
        Trajectory[] trajectories;
        Event[] events;
        List<ExThreat> exThreats;
        int turn;
        int phase;
        int score;
        bool[] phaseComputer;

        int[] observation;
        int observationCount;
        int[] obsBonus = new int[] { 0, 1, 2, 3, 5, 7, 9, 12, 15, 18, 22 };

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
            ship = new Ship(players);
            exThreats = new List<ExThreat>();
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
                        foreach(Player p in players)
                        {
                            if (p.position < 6)
                                p.Delay(turn-1);
                        }
                    }
                }

                //Check event
                if(eventIdx < events.Length && events[eventIdx].turn == turn)
                {
                    //Summon threat
                    Event ev = events[eventIdx];
                    eventIdx++;
                    if(ev.external)
                        exThreats.Add(ThreatFactory.SummonEx(ev.creature,trajectories[ev.zone],ev.zone, ship));
                }

                //Perform player actions
                playerActions(turn - 1);

                //Rocket hits
                if (ship.rocketReady)
                {
                    ship.rocketReady = false;
                    int target = -1;
                    int distance = int.MaxValue;
                    for (int i = 0; i < exThreats.Count; i++)
                    {
                        //Check if valid target
                        if(!exThreats[i].rocketImmune && exThreats[i].distanceRange <= 2 && exThreats[i].distance < distance)
                        {
                            target = i;
                            distance = exThreats[i].distance;
                        }
                    }
                    //Deal damage
                    if(target != -1)
                        exThreats[target].DealDamage(3, 2, DmgSource.rocket);
                }
                

                //Process damage
                for (int i = 0; i < exThreats.Count; i++)
                    exThreats[i].ProcessDamage();

                //Cleanup dead
                for(int i = exThreats.Count - 1; i >= 0; i--)
                {
                    //Threat is gone
                    if (exThreats[i].beaten)
                    {
                        if (exThreats[i].alive)
                            score += exThreats[i].scoreLose;
                        else
                            score += exThreats[i].scoreWin;
                        exThreats.RemoveAt(i);
                    }
                }

                //Move threats
                for (int i = 0; i < exThreats.Count; i++)
                    exThreats[i].Move();

                //Move rocket
                if (ship.rocketFired)
                {
                    ship.rocketReady = true;
                    ship.rocketFired = false;
                }

                //Calculate observation bonus
                observation[phase] = Math.Max(observation[phase], obsBonus[observationCount]);

                //Check if gameover
                for(int i = 0; i < 3; i++)
                {
                    if (ship.damage[i] >= 7)
                        gameover = true;
                }
                if (gameover)
                    break;

                turn++;
            }

            //Count damage
            int highestDamage = 0;
            for(int i = 0; i < 3; i++)
            {
                score -= ship.damage[i];
                highestDamage = Math.Max(ship.damage[i], highestDamage);
            }
            score -= highestDamage;

            //Count dead crew
            foreach(Player p in players)
            {
                if (!p.alive)
                    score -= 2;
            }

            //Observation bonus
            if(!gameover)
                score += observation[0]+observation[1]+observation[2];

            //Gameover penalty
            if (gameover)
                score = score - 200 + turn;

            return score;
        }


        //All players perform actions
        void playerActions(int t)
        {
            //Loop over players
            for(int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                p.lastAction = t;
                Act a = p.actions[t];
                int z = p.position % 3;

                if (!p.alive)
                    continue;
                //Add special inInterceptor case

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

                        if(p.position < 3)
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
                                if(et.zone == z && et.distance < distance)
                                {
                                    target = j;
                                    distance = et.distance;
                                }
                            }
                            //Deal damage
                            if(target != -1)
                                exThreats[target].DealDamage(ship.laserDamage[z], 3, DmgSource.laser);
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
                                    et.DealDamage(1, ship.pulseRange, DmgSource.impulse);
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
                                if(target != -1)
                                    exThreats[target].DealDamage(ship.plasmaDamage[z], 3, DmgSource.plasma);
                            }
                        }
                        break;

                    case Act.B:
                        //Refill shield
                        if(p.position < 3)
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
                            if(z == 1)
                            {
                                //Fill main
                                if(ship.capsules > 0)
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
                            //Computer
                            case 1:
                                phaseComputer[phase] = true;
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
                        break;
                    case Act.empty:
                        break;
                }
            }
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
