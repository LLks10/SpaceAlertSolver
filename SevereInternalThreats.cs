using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    //count: 5

    class CommandosRed : InThreat
    {
        public CommandosRed(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 3;
            speed = 2;
            scoreLose = 4;
            scoreWin = 8;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public override void ActX()
        {
            if (position < 3)
                position += 3;
            else
                position -= 3;

        }
        public override void ActY()
        {
            if(health < 2)
            {
                if (position != 2 && position != 5)
                    position++;
            }
            else
            {
                int z = position % 3;
                ship.DealDamageIntern(z, 2);
            }            
        }
        public override void ActZ()
        {
            int z = position % 3;
            ship.DealDamageIntern(z, 4);
            for(int i = 0; i < ship.players.Length; i++)
            {
                if (ship.players[i].position == position)
                    ship.players[i].alive = false;
            }
        }
    }

    class CommandosBlue : InThreat
    {
        public CommandosBlue(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 2;
            speed = 2;
            scoreLose = 4;
            scoreWin = 8;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public override void ActX()
        {
            if (position < 3)
                position += 3;
            else
                position -= 3;

        }
        public override void ActY()
        {
            if (health < 2)
            {
                if (position != 0 && position != 3)
                    position--;
            }
            else
            {
                int z = position % 3;
                ship.DealDamageIntern(z, 2);
            }
        }
        public override void ActZ()
        {
            int z = position % 3;
            ship.DealDamageIntern(z, 4);
            for (int i = 0; i < ship.players.Length; i++)
            {
                if (ship.players[i].position == position)
                    ship.players[i].alive = false;
            }
        }
    }

    class Alien : InThreat
    {
        public Alien(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 4;
            speed = 2;
            scoreLose = 0;
            scoreWin = 8;
            vulnerability = InDmgSource.android;
        }

        public override void ActX()
        {
            fightBack = true;
        }
        public override void ActY()
        {
            if (position < 3)
                position += 3;
            else
                position -= 3;
            int c = 0;
            for(int i = 0; i < ship.players.Length; i++)
            {
                if (ship.players[i].position == position)
                    c++;
            }
            ship.DealDamageIntern(position % 3, c);
        }
        public override void ActZ()
        {
            ship.damage[0] = 7;
            ship.damage[1] = 7;
            ship.damage[2] = 7;
        }
    }

    class Eliminator : InThreat
    {
        public Eliminator(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 2;
            speed = 2;
            scoreLose = 6;
            scoreWin = 12;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public override void ActX()
        {
            if (position != 0 && position != 3)
                position--;
            KillAll();
        }
        public override void ActY()
        {
            if (position < 3)
                position += 3;
            else
                position -= 3;
            KillAll();
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(position % 3, 3);
        }

        private void KillAll()
        {
            for(int i = 0; i < ship.players.Length; i++)
            {
                if (ship.players[i].team == null || ship.players[i].team.alive == false)
                    ship.players[i].alive = false;
            }
        }
    }

    class SearchRobot : InThreat
    {
        public SearchRobot(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 1;
            speed = 2;
            scoreLose = 6;
            scoreWin = 15;
            vulnerability = InDmgSource.android;
        }

        public override void ActX()
        {
            MoveToClosest();
        }
        public override void ActY()
        {
            MoveToClosest();
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(position % 3, 5);
            for(int i = 0; i < ship.players.Length; i++)
            {
                if (ship.players[i].position == position)
                    ship.players[i].alive = false;
            }
        }

        public override bool DealDamage(int position, InDmgSource source)
        {
            if (source == vulnerability && AtPosition(position))
            {
                health--;
                if (health <= 0)
                {
                    //Kill killing player
                    int highestAct = -1;
                    int score = int.MinValue;
                    //Find player that did most recent action
                    for(int i = 0; i < ship.players.Length; i++)
                    {
                        if(ship.players[i].lastAction >= score)
                        {
                            highestAct = i;
                            score = ship.players[i].lastAction;
                        }
                    }
                    ship.players[highestAct].alive = false;
                    ship.players[highestAct].team.alive = false;

                    alive = false;
                    beaten = true;
                }
                return true;
            }
            return false;
        }

        private void MoveToClosest()
        {
            int[] nearbyStation;
            switch (position)
            {
                case 0:
                    nearbyStation = new int[] { 1, 3 };
                    break;
                case 1:
                    nearbyStation = new int[] { 0, 4, 2 };
                    break;
                case 2:
                    nearbyStation = new int[] { 1, 5 };
                    break;
                case 3:
                    nearbyStation = new int[] { 0, 4 };
                    break;
                case 4:
                    nearbyStation = new int[] { 3, 1, 5 };
                    break;
                default: //case 5
                    nearbyStation = new int[] { 2, 4 };
                    break;
            }
            int best = -1;
            int score = int.MinValue;
            Player[] ps = ship.players;
            //Count players and choose best station
            for(int i = 0; i < nearbyStation.Length; i++)
            {
                int c = 0;
                //Count players
                for(int j = 0; j < ps.Length; j++)
                {
                    if (ps[j].position == nearbyStation[i])
                        c++;
                }
                if (c > score)
                {
                    best = i;
                    score = c;
                }
                else if (c == score)
                    best = -1;
            }

            if (best != -1)
                position = nearbyStation[best];
        }
    }
}
