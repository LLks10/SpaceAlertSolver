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
        public int health, shield, speed, distance , distanceRange, scoreWin, scoreLose, zone;
        public Trajectory trajectory;
        public bool rocketImmune, alive, beaten;
        public Ship ship;
        public int damage;

        public ExThreat(Ship ship, Trajectory traj, int zone)
        {
            this.ship = ship;
            this.trajectory = traj;
            this.zone = zone;
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

        public virtual void DealDamage(int damage, int range, DmgSource source)
        {
            if(range >= distanceRange)
                this.damage += damage; 
        }

        public virtual bool ProcessDamage()
        {
            health = health - (damage - shield);
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

        public virtual void ActX() { }
        public virtual void ActY() { }
        public virtual void ActZ() { }
    }

    //Create threats
    public static class ThreatFactory
    {
        //External threat
        public static ExThreat SummonEx(int number,Trajectory traj, int zone, Ship ship)
        {
            switch (number)
            {
                case 0:
                    return new ArmoredCatcher(ship, traj, zone);
                case 1:
                    return new Amoebe(ship, traj, zone);
                case 2:
                    return new Battleship(ship, traj, zone);
                case 3:
                    return new Hunter(ship, traj, zone);
                case 4:
                    return new GyroHunter(ship, traj, zone);
                case 5:
                    return new EnergyCloud(ship, traj, zone);
                case 6:
                    return new Meteorite(ship, traj, zone);
                case 7:
                    return new ImpulseBall(ship, traj, zone);
                case 8:
                    return new SpaceCruiser(ship, traj, zone);
                case 9:
                    return new StealthHunter(ship, traj, zone);
                case 10:
                    return new JellyFish(ship, traj, zone);
                case 11:
                    return new SmallAsteroid(ship, traj, zone);
                case 12:
                    return new Kamikaze(ship, traj, zone);
                case 13:
                    return new Swarm(ship, traj, zone);
                case 14:
                    return new GhostHunter(ship, traj, zone);
                case 15:
                    return new Fregat(ship, traj, zone);
                case 16:
                    return new GyroFregat(ship, traj, zone);
                case 17:
                    return new WarDeck(ship, traj, zone);
                case 18:
                    return new InterStellarOctopus(ship, traj, zone);
                case 19:
                    return new Maelstorm(ship, traj, zone);
                case 20:
                    return new Asteroid(ship, traj, zone);
                case 21:
                    return new ImpulseSatellite(ship, traj, zone);
                case 22:
                    return new Nemesis(ship, traj, zone);
                case 23:
                    return new NebulaCrab(ship, traj, zone);
                case 24:
                    return new PsionicSatellite(ship, traj, zone);
                case 25:
                    return new LargeAsteroid(ship, traj, zone);
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
                    return "GyroHunter";
                case 5:
                    return "EnergyCloud";
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
                    return "Fregat";
                case 16:
                    return "Gyro Fregat";
                case 17:
                    return "Wardeck";
                case 18:
                    return "Interstellar Octopus";
                case 19:
                    return "Maelstorm";
                case 20:
                    return "Asteroid";
                case 21:
                    return "Impulse Satellite";
                case 22:
                    return "Nemesis";
                case 23:
                    return "Nebula Crab";
                case 24:
                    return "Psionic Satellite";
                case 25:
                    return "Large Asteroid";
            }
            return "";
        }
    }

    //Damage sources
    public enum DmgSource
    {
        laser,
        plasma,
        impulse,
        rocket,
        intercept
    }
}
