using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    //External threat parent class
    public abstract class ExThreat
    {
        public int health, shield, speed, distance , distanceRange, scoreWin, scoreLose, zone, time;
        public Trajectory trajectory;
        public bool rocketImmune, alive, beaten;
        public Ship ship;
        public int damage;

        public ExThreat(Ship ship, Trajectory traj, int zone, int time)
        {
            this.ship = ship;
            this.trajectory = traj;
            this.zone = zone;
            this.time = time;
            alive = true;

            //Get trajectory information
            distance = traj.maxDistance;

            //Set new distance range
            if (distance <= trajectory.dist1)
                distanceRange = 1;
            else if (distance <= trajectory.dist2)
                distanceRange = 2;
            else
                distanceRange = 3;
        }

        public virtual void DealDamage(int damage, int range, ExDmgSource source)
        {
            if(range >= distanceRange)
                this.damage += damage; 
        }

        public virtual bool ProcessDamage()
        {
            health = health - Math.Max(0,(damage - shield));
            damage = 0;

            if (health <= 0)
            {
                alive = false;
                beaten = true;
                return true;
            }
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
                beaten = true;

            //Set new distance range
            if (distance <= trajectory.dist1)
                distanceRange = 1;
            else if (distance <= trajectory.dist2)
                distanceRange = 2;
            else
                distanceRange = 3;
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

            //Set new distance range
            if (distance <= trajectory.dist1)
                distanceRange = 1;
            else if (distance <= trajectory.dist2)
                distanceRange = 2;
            else
                distanceRange = 3;
        }

        public virtual void ActX() { }
        public virtual void ActY() { }
        public virtual void ActZ() { }
    }

    //Damage sources
    public enum ExDmgSource
    {
        laser,
        plasma,
        impulse,
        rocket,
        intercept
    }
}
