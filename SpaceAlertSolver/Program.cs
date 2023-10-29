namespace SpaceAlertSolver;

/// <summary>
/// Set the number of players in this file by changing <see cref="NUM_AI_PLAYERS"/>.
/// Set the number of human players in the <see cref="Gene"/> file.
/// </summary>
class Program
{
    public const int SEED = 8;
    public const bool PRINT_DEBUG = true;
    public const int NUM_AI_PLAYERS = 4;
    public const int NUM_SIMULATIONS = 5000000;

    static void Main(string[] args)
    {
        if (args.Length > 0 && int.TryParse(args[0], out int seed))
        {
            Run(seed);
        }
        else
        {
            Run(SEED);
        }

        /*int seed_start = 10;
        int seed_end = 20;

        double[] scores = new double[10];
        for (int seed = seed_start; seed < seed_end; seed++)
        {
            scores[seed-seed_start] = Run(seed).getScore();
        }
        for (int i = 0; i < scores.Length; i++)
        {
            Console.WriteLine($"{i + seed_start}: {scores[i]}");
        }*/

        Console.ReadLine();
    }

    public static void PrintStats(List<int> stats)
    {
        stats.Sort();
        int winStart = stats.BinarySearch(-40);
        if (winStart < 0)
            winStart = ~winStart;
        else
            winStart++;

        double average = 0;
        for (int i = 0; i < stats.Count; i++)
        {
            average += (double)stats[i] / stats.Count;
        }

        double median;
        if (stats.Count % 2 == 0)
        {
            median = stats[stats.Count / 2] / 2.0 + stats[stats.Count / 2 - 1] / 2.0;
        }
        else
        {
            median = stats[stats.Count / 2];
        }

        Console.WriteLine($"Wins: {stats.Count - winStart} | Losses: {winStart}");
        Console.WriteLine($"Max: {stats[stats.Count-1]} | Min: {stats[0]}");
        Console.WriteLine($"Mean: {average} | Median: {median}");
    }

    static Gene Run(int seed)
    {
        //Extension.InitKeys(5, 8, 1 << 25);
        //Create damage order
        Random r = new Random(seed);
        List<Event> events = new List<Event>();

        //Setup threat pool
        int comExThreatCount = 16;
        List<int> comExThreats = new List<int>();
        for (int i = 0; i < comExThreatCount; i++)
            comExThreats.Add(i);
        int sevExThreatCount = 13;
        List<int> sevExThreats = new List<int>();
        for (int i = 0; i < sevExThreatCount; i++)
            sevExThreats.Add(i + comExThreatCount);
        int comInThreatCount = 13;
        List<int> comInThreats = new List<int>();
        for (int i = 0; i < comInThreatCount; i++)
            comInThreats.Add(i);
        int sevInThreatCount = 11;
        List<int> sevInThreats = new List<int>();
        for (int i = 0; i < sevInThreatCount; i++)
            sevInThreats.Add(i + comInThreatCount);

        //Load trajectories
        Trajectory[] trajectories = new Trajectory[4];
        int[] ts = new int[] { 0, 1, 2, 3, 4, 5, 6 };

        Console.WriteLine("'r' for random trajectories or xxxx to manually set");
        string resp = Console.ReadLine();
        if (resp == "r" || resp == "R")
        {
            for (int i = 0; i < 4; i++)
            {
                int tsi = -1;
                while (tsi == -1)
                    tsi = ts[r.Next(7)];
                trajectories[i] = new Trajectory(tsi);
                ts[tsi] = -1;
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
                trajectories[i] = new Trajectory(int.Parse(resp[i].ToString()) - 1);
        }

        Console.WriteLine("Trajectories:");
        Console.WriteLine("Lt{0} Mt{1} Rt{2} It{3}", trajectories[0].number + 1, trajectories[1].number + 1, trajectories[2].number + 1, trajectories[3].number + 1);
        Console.WriteLine();
        Console.WriteLine("Add events: 'turn 1...12' 'zone 0,1,2,3' ('name' | 'r' 'severity 0,1') | Type 'start' to start simulation");
        string[] zoneStr = new string[] { "Red", "White", "Blue", "Internal" };
        while (true)
        {
            string str = Console.ReadLine();
            if (str == "start")
                break;
            //Get threat
            else
            {
                string[] strsplit = str.Split(' ');
                int t = int.Parse(strsplit[0]);
                int z = int.Parse(strsplit[1]);
                
                // name given
                if (strsplit[2] == "r")
                {
                    int thrt;
                    string name;
                    if (z < 3)
                    {
                        if(strsplit[3] == "0")
                        {
                            int thrtIdx = r.Next(comExThreats.Count);
                            thrt = comExThreats[thrtIdx];
                            comExThreats.RemoveAt(thrtIdx);
                        }
                        else
                        {
                            int thrtIdx = r.Next(sevExThreats.Count);
                            thrt = sevExThreats[thrtIdx];
                            sevExThreats.RemoveAt(thrtIdx);
                        }

                        name = ThreatFactory.ExName(thrt);
                    }
                    else
                    {
                        if (strsplit[3] == "0")
                        {
                            int thrtIdx = r.Next(comInThreats.Count);
                            thrt = comInThreats[thrtIdx];
                            comInThreats.RemoveAt(thrtIdx);
                        }
                        else
                        {
                            int thrtIdx = r.Next(sevInThreats.Count);
                            thrt = sevInThreats[thrtIdx];
                            sevInThreats.RemoveAt(thrtIdx);
                        }

                        name = ThreatFactory.InName(thrt);
                    }

                    events.Add(new Event(z < 3, t, z, thrt));
                    Console.WriteLine("Loaded {0} on turn {1} in zone {2} ({3})", name, t, zoneStr[z], thrt);
                }
                else
                {
                    string name = strsplit[2];
                    for (int i = 3; i < strsplit.Length; i++)
                    {
                        name += ' ' + strsplit[i];
                    }

                    ThreatName tn;
                    // external
                    if (z < 3)
                    {
                        tn = ThreatParser.ParseExThreat(name);
                    }
                    else
                    {
                        tn = ThreatParser.ParseInThreat(name);
                    }

                    events.Add(new Event(tn.external, t, z, tn.id));
                    Console.WriteLine("Loaded {0} on turn {1} in zone {2} ({3})", tn.name, t, zoneStr[z], tn.id);
                }
            }
        }
        Event[] evArr = events.OrderBy(e => e.Turn).ToArray();

        //Random simulations
        /*Genetic genetic = new Genetic(400, 5, trajectories, evArr);
        while (true)
        {
            int sims = 0;
            while (sims < 10000)
            {
                genetic.Cycle(trajectories, evArr);
                sims++;
            }
            Console.WriteLine("Continue? (Y/N)  Reset: R");
            string ans = Console.ReadLine();
            if (ans == "N" || ans == "n")
                break;
            if (ans == "R" || ans == "r")
                genetic = new Genetic(400, 5, trajectories, evArr);
        }
        Console.ReadLine();*/

        SimulatedAnnealing sa = new SimulatedAnnealing(NUM_AI_PLAYERS, trajectories, evArr);
        sa.Run(NUM_SIMULATIONS, trajectories, evArr, seed);
        if (PRINT_DEBUG)
        {
            Console.WriteLine(sa.getBestGene().Rep() + sa.getBestGene().getScore());
            Console.WriteLine("-----FINAL-----");
            PrintStats(sa.getBestStats());
        }
        else
        {
            Console.WriteLine(sa.getBestGene().Rep());
            Console.WriteLine("-----FINAL-----");
        }

        return sa.getBestGene();
    }
}
