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

            if (health < 0)
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

    //Create threats
    public static class ThreatFactory
    {
        //External threat
        public static ExThreat SummonEx(int number,Trajectory traj, int zone, Ship ship, int time)
        {
            switch (number)
            {
                case 0:
                    return new  ArmoredCatcher(ship, traj, zone, time);
                case 1:                                
                    return new          Amoebe(ship, traj, zone, time);
                case 2:                                      
                    return new      Battleship(ship, traj, zone, time);
                case 3:                                       
                    return new          Hunter(ship, traj, zone, time);
                case 4:                                      
                    return new      GyroHunter(ship, traj, zone, time);
                case 5:                                       
                    return new     EnergyCloud(ship, traj, zone, time);
                case 6:                                       
                    return new       Meteorite(ship, traj, zone, time);
                case 7:                                      
                    return new     ImpulseBall(ship, traj, zone, time);
                case 8:                                   
                    return new    SpaceCruiser(ship, traj, zone, time);
                case 9:                                     
                    return new   StealthHunter(ship, traj, zone, time);
                case 10:                                    
                    return new       JellyFish(ship, traj, zone, time);
                case 11:                                   
                    return new   SmallAsteroid(ship, traj, zone, time);
                case 12:                                   
                    return new        Kamikaze(ship, traj, zone, time);
                case 13:                                  
                    return new           Swarm(ship, traj, zone, time);
                case 14:                                 
                    return new     GhostHunter(ship, traj, zone, time);
                case 15:
                    return new Scout(ship, traj, zone, time);
                case 16:                                   
                    return new          Fregat(ship, traj, zone, time);
                case 17:                                    
                    return new      GyroFregat(ship, traj, zone, time);
                case 18:                                     
                    return new         WarDeck(ship, traj, zone, time);
                case 19:
                    return new InterStellarOctopus(ship, traj, zone, time);
                case 20:
                    return new      Maelstorm(ship, traj, zone, time);
                case 21:
                    return new      Asteroid(ship, traj, zone, time);
                case 22:
                    return new      ImpulseSatellite(ship, traj, zone, time);
                case 23:
                    return new      Nemesis(ship, traj, zone, time);
                case 24:
                    return new      NebulaCrab(ship, traj, zone, time);
                case 25:
                    return new      PsionicSatellite(ship, traj, zone, time);
                case 26:
                    return new      LargeAsteroid(ship, traj, zone, time);
                case 27:
                    return new Moloch(ship, traj, zone, time);
            }
            return null;
        }

        public static string ExName(int number)
        {
            switch (number)
            {
                case 0:
                    return "Armored catcher";
                case 1:
                    return "Amoebe";
                case 2:
                    return "Battleship";
                case 3:
                    return "Hunter";
                case 4:
                    return "Gyro Hunter";
                case 5:
                    return "Energy Cloud";
                case 6:
                    return "Meteorite";
                case 7:
                    return "Impulse Ball";
                case 8:
                    return "Space Cruiser";
                case 9:
                    return "Stealth Hunter";
                case 10:
                    return "Jellyfish";
                case 11:
                    return "Small Asteroid";
                case 12:
                    return "Kamikaze";
                case 13:
                    return "Swarm";
                case 14:
                    return "Ghost Hunter";
                case 15:
                    return "Scout";
                case 16:
                    return "Fregat";
                case 17:
                    return "Gyro Fregat";
                case 18:
                    return "Wardeck";
                case 19:
                    return "Interstellar Octopus";
                case 20:
                    return "Maelstorm";
                case 21:
                    return "Asteroid";
                case 22:
                    return "Impulse Satellite";
                case 23:
                    return "Nemesis";
                case 24:
                    return "Nebula Crab";
                case 25:
                    return "Psionic Satellite";
                case 26:
                    return "Large Asteroid";
                case 27:
                    return "Moloch";
            }
            return "";
        }
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
