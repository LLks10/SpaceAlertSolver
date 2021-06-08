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

        bool started_move = false;
        int distance_moved;
        int current_speed;

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

        public ExThreat() { }

        public abstract ExThreat Clone(Ship ship);

        protected virtual void CloneThreat(ExThreat other, Ship ship)
        {
            health = other.health;
            shield = other.shield;
            speed = other.speed;
            distance = other.distance;
            distanceRange = other.distanceRange;
            scoreWin = other.scoreWin;
            scoreLose = other.scoreLose;
            zone = other.zone;
            time = other.time;
            trajectory = other.trajectory;
            rocketImmune = other.rocketImmune;
            alive = other.alive;
            beaten = other.beaten;
            damage = other.damage;

            started_move = other.started_move;
            distance_moved = other.distance_moved;
            current_speed = other.current_speed;

            this.ship = ship;
        }

        public virtual int GetDistance(int range, ExDmgSource source)
        {
            if (range >= distanceRange && !beaten && !(source == ExDmgSource.rocket && rocketImmune))
                return distance;
            return 100;
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
            distance -= current_speed;
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
