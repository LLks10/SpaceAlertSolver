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

        public InThreat(Ship ship, Trajectory traj, int time)
        {
            this.ship = ship;
            this.trajectory = traj;
            this.time = time;
            alive = true;

            //Get trajectory information
            distance = traj.maxDistance;
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
            //Play actions
            int curSpd = speed;
            for(int i = distance-1; i >= distance-curSpd && i >= 0; i--)
            {
                if (trajectory.actions[i] == 1)
                    ActX();
                else if (trajectory.actions[i] == 2)
                    ActY();
                else if (trajectory.actions[i] == 3)
                    ActZ();
            }

            //Set new position
            distance -= curSpd;
            if (distance <= 0)
            {
                OnClear();
                beaten = true;
            }
        }

        public virtual void Move(int mSpd)
        {
            //Play actions
            for (int i = distance - 1; i >= distance - mSpd && i >= 0; i--)
            {
                if (trajectory.actions[i] == 1)
                    ActX();
                else if (trajectory.actions[i] == 2)
                    ActY();
                else if (trajectory.actions[i] == 3)
                    ActZ();
            }

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
