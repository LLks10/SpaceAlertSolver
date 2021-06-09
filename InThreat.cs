using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    //External threat parent class
    public abstract class InThreat
    {
        public int health, speed, distance, position, scoreWin, scoreLose, time;
        public Trajectory trajectory;
        public InDmgSource vulnerability;
        public bool alive, beaten, fightBack;
        public Ship ship;

        bool started_move = false;
        int distance_moved;
        int current_speed;

        public InThreat(Ship ship, Trajectory traj, int time)
        {
            this.ship = ship;
            this.trajectory = traj;
            this.time = time;
            alive = true;

            //Get trajectory information
            distance = traj.maxDistance;
        }
        public InThreat() { }

        public abstract InThreat Clone(Ship ship);

        protected virtual void CloneThreat(InThreat other, Ship ship)
        {
            health = other.health;
            speed = other.speed;
            distance = other.distance;
            position = other.position;
            scoreWin = other.scoreWin;
            scoreLose = other.scoreLose;
            time = other.time;
            trajectory = other.trajectory;
            vulnerability = other.vulnerability;
            alive = other.alive;
            beaten = other.beaten;
            fightBack = other.fightBack;

            started_move = other.started_move;
            distance_moved = other.distance_moved;
            current_speed = other.current_speed;

            this.ship = ship;
        }

        public virtual bool DealDamage(int position, InDmgSource source)
        {
            if(source == vulnerability && AtPosition(position))
            {
                health--;
                if(health <= 0)
                {
                    alive = false;
                    beaten = true;
                    OnClear();
                }
                return true;
            }
            return false;
        }

        public virtual void OnClear() { }

        public virtual bool AtPosition(int position)
        {
            return position == this.position;
        }

        public virtual bool ProcessTurnEnd()
        {
            return false;
        }

        public virtual void Move()
        {
            Move(speed);
        }

        public virtual void Move(int mSpd)
        {
            if (!started_move)
            {
                current_speed = mSpd;
                distance_moved = 0;
                started_move = true;
            }
            while (distance_moved < current_speed && distance - distance_moved > 0)
            {
                switch (trajectory.actions[distance - distance_moved - 1])
                {
                    case 1:
                        ActX();
                        break;
                    case 2:
                        ActY();
                        break;
                    case 3:
                        ActZ();
                        break;
                }
                distance_moved++;
            }
            started_move = false;

            //Set new position
            distance -= mSpd;
            if (distance <= 0)
                beaten = true;
        }

        public virtual void ActX() { }
        public virtual void ActY() { }
        public virtual void ActZ() { }
    }

    //Damage sources
    public enum InDmgSource
    {
        B,
        C,
        android,
    }
}
