using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    //Create threats
    public static class ThreatFactory
    {
        const int sevInStart = 0;
        //External threat
        public static ExThreat SummonEx(int number, Trajectory traj, int zone, Ship ship, int time)
        {
            switch (number)
            {
                case 0:
                    return new ArmoredCatcher(ship, traj, zone, time);
                case 1:
                    return new Amoebe(ship, traj, zone, time);
                case 2:
                    return new Battleship(ship, traj, zone, time);
                case 3:
                    return new Hunter(ship, traj, zone, time);
                case 4:
                    return new GyroHunter(ship, traj, zone, time);
                case 5:
                    return new EnergyCloud(ship, traj, zone, time);
                case 6:
                    return new Meteorite(ship, traj, zone, time);
                case 7:
                    return new ImpulseBall(ship, traj, zone, time);
                case 8:
                    return new SpaceCruiser(ship, traj, zone, time);
                case 9:
                    return new StealthHunter(ship, traj, zone, time);
                case 10:
                    return new JellyFish(ship, traj, zone, time);
                case 11:
                    return new SmallAsteroid(ship, traj, zone, time);
                case 12:
                    return new Kamikaze(ship, traj, zone, time);
                case 13:
                    return new Swarm(ship, traj, zone, time);
                case 14:
                    return new GhostHunter(ship, traj, zone, time);
                case 15:
                    return new Scout(ship, traj, zone, time);
                case 16:
                    return new Fregat(ship, traj, zone, time);
                case 17:
                    return new GyroFregat(ship, traj, zone, time);
                case 18:
                    return new WarDeck(ship, traj, zone, time);
                case 19:
                    return new InterStellarOctopus(ship, traj, zone, time);
                case 20:
                    return new Maelstorm(ship, traj, zone, time);
                case 21:
                    return new Asteroid(ship, traj, zone, time);
                case 22:
                    return new ImpulseSatellite(ship, traj, zone, time);
                case 23:
                    return new Nemesis(ship, traj, zone, time);
                case 24:
                    return new NebulaCrab(ship, traj, zone, time);
                case 25:
                    return new PsionicSatellite(ship, traj, zone, time);
                case 26:
                    return new LargeAsteroid(ship, traj, zone, time);
                case 27:
                    return new Moloch(ship, traj, zone, time);
                case 28:
                    return new Behemoth(ship, traj, zone, time);
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
                case 28:
                    return "Behemoth";
            }
            return "";
        }

        //Internal threat
        public static InThreat SummonIn(int number, Trajectory traj, Ship ship, int time)
        {
            switch (number)
            {
                case 0:
                    return new SaboteurRed(ship, traj, time);
                case 1:
                    return new SaboteurBlue(ship, traj, time);
                case 2:
                    return new SkirmisherRed(ship, traj, time);
                case 3:
                    return new SkirmisherBlue(ship, traj, time);
            }
            return null;
        }

        public static string InName(int number)
        {
            switch (number)
            {
                case 0:
                    return "Red Saboteur";
                case 1:
                    return "Blue Saboteur";
                case 2:
                    return "Red Skirmisher";
                case 3:
                    return "Blue Skirmisher";
            }
            return "";
        }
    }
}
