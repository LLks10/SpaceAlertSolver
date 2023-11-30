namespace SpaceAlertSolver;

//Create threats
public static class ThreatFactory
{
    public static List<int> GetInternalThreatPool()
    {
        List<int> pool = new();
        for (int i = 0; i <= 23; i++)
        {
            pool.Add(i);
        }
        return pool;
    }

    public static List<int> GetExternalThreatPool()
    {
        List<int> pool = new();
        for (int i = 0; i <= 28; i++)
        {
            pool.Add(i);
        }
        return pool;
    }

    const int sevInStart = 13;
    //External threat
    internal static ExThreat SummonEx(int number, Trajectory traj, int zone, Game game, int time)
    {
        switch (number)
        {
            case 0:
                return new ArmoredCatcher(game, traj, zone, time);
            case 1:
                return new Amoebe(game, traj, zone, time);
            case 2:
                return new Battleship(game, traj, zone, time);
            case 3:
                return new Hunter(game, traj, zone, time);
            case 4:
                return new GyroHunter(game, traj, zone, time);
            case 5:
                return new EnergyCloud(game, traj, zone, time);
            case 6:
                return new Meteorite(game, traj, zone, time);
            case 7:
                return new ImpulseBall(game, traj, zone, time);
            case 8:
                return new SpaceCruiser(game, traj, zone, time);
            case 9:
                return new StealthHunter(game, traj, zone, time);
            case 10:
                return new JellyFish(game, traj, zone, time);
            case 11:
                return new SmallAsteroid(game, traj, zone, time);
            case 12:
                return new Kamikaze(game, traj, zone, time);
            case 13:
                return new Swarm(game, traj, zone, time);
            case 14:
                return new GhostHunter(game, traj, zone, time);
            case 15:
                return new Scout(game, traj, zone, time);
            case 16:
                return new Fregat(game, traj, zone, time);
            case 17:
                return new GyroFregat(game, traj, zone, time);
            case 18:
                return new WarDeck(game, traj, zone, time);
            case 19:
                return new InterStellarOctopus(game, traj, zone, time);
            case 20:
                return new Maelstorm(game, traj, zone, time);
            case 21:
                return new Asteroid(game, traj, zone, time);
            case 22:
                return new ImpulseSatellite(game, traj, zone, time);
            case 23:
                return new Nemesis(game, traj, zone, time);
            case 24:
                return new NebulaCrab(game, traj, zone, time);
            case 25:
                return new PsionicSatellite(game, traj, zone, time);
            case 26:
                return new LargeAsteroid(game, traj, zone, time);
            case 27:
                return new Moloch(game, traj, zone, time);
            case 28:
                return new Behemoth(game, traj, zone, time);
        }
        return null;
    }

    public static string ExName(int number)
    {
        switch (number)
        {
            case 0:
                return "Armored Grappler";
            case 1:
                return "Amoebe";
            case 2:
                return "Gunship";
            case 3:
                return "Fighter";
            case 4:
                return "Gyro Fighter";
            case 5:
                return "Energy Cloud";
            case 6:
                return "Meteorite";
            case 7:
                return "Pulse Ball";
            case 8:
                return "Destroyer";
            case 9:
                return "Stealth Fighter";
            case 10:
                return "Jellyfish";
            case 11:
                return "Minor Asteroid";
            case 12:
                return "Kamikaze";
            case 13:
                return "Swarm";
            case 14:
                return "Phantom Fighter";
            case 15:
                return "Scout";
            case 16:
                return "Fregat";
            case 17:
                return "Gyro Fregat";
            case 18:
                return "Man-o-War";
            case 19:
                return "Interstellar Octopus";
            case 20:
                return "Maelstorm";
            case 21:
                return "Asteroid";
            case 22:
                return "Pulse Satellite";
            case 23:
                return "Nemesis";
            case 24:
                return "Nebula Crab";
            case 25:
                return "Psionic Satellite";
            case 26:
                return "Major Asteroid";
            case 27:
                return "Moloch";
            case 28:
                return "Behemoth";
        }
        return "";
    }

    //Internal threat
    internal static InThreat SummonIn(int number, Trajectory traj, Game game, int time)
    {
        switch (number)
        {
            case 0:
                return new SaboteurRed(game, traj, time);
            case 1:
                return new SaboteurBlue(game, traj, time);
            case 2:
                return new SkirmisherRed(game, traj, time);
            case 3:
                return new SkirmisherBlue(game, traj, time);
            case 4:
                return new SoldiersRed(game, traj, time);
            case 5:
                return new SoldiersBlue(game, traj, time);
            case 6:
                return new Virus(game, traj, time);
            case 7:
                return new HackedShieldsRed(game, traj, time);
            case 8:
                return new HackedShieldsBlue(game, traj, time);
            case 9:
                return new OverheatedReactor(game, traj, time);
            case 10:
                return new UnstableWarheads(game, traj, time);
            case 11:
                return new SlimeBlue(game, traj, time);
            case 12:
                return new SlimeRed(game, traj, time);

            case sevInStart:
                return new CommandosRed(game, traj, time);
            case sevInStart + 1:
                return new CommandosBlue(game, traj, time);
            case sevInStart + 2:
                return new Alien(game, traj, time);
            case sevInStart + 3:
                return new Eliminator(game, traj, time);
            case sevInStart + 4:
                return new SearchRobot(game, traj, time);
            case sevInStart + 5:
                return new AtomicBomb(game, traj, time);
            case sevInStart + 6:
                return new RebelliousRobots(game, traj, time);
            case sevInStart + 7:
                return new SwitchedCables(game, traj, time);
            case sevInStart + 8:
                return new OverstrainedEnergyNet(game, traj, time);
            case sevInStart + 9:
                return new Fissure(game, traj, time);
            case sevInStart + 10:
                return new Infection(game, traj, time);
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
            case 4:
                return "Red Soldiers";
            case 5:
                return "Blue Soldiers";
            case 6:
                return "Virus";
            case 7:
                return "Red Hacked Shields";
            case 8:
                return "Blue Hacked Shields";
            case 9:
                return "Overheated Reactor";
            case 10:
                return "Unstable Warheads";
            case 11:
                return "Blue Slime";
            case 12:
                return "Red Slime";

            case sevInStart:
                return "Red Commandos";
            case sevInStart + 1:
                return "Blue Commandos";
            case sevInStart + 2:
                return "Alien";
            case sevInStart + 3:
                return "Eliminator";
            case sevInStart + 4:
                return "Seeker";
            case sevInStart + 5:
                return "Atomic Bomb";
            case sevInStart + 6:
                return "Rebellious Robots";
            case sevInStart + 7:
                return "Crossed Wire";
            case sevInStart + 8:
                return "Power System Overload";
            case sevInStart + 9:
                return "Fissure";
            case sevInStart + 10:
                return "Contamination";
        }
        return "";
    }
}
