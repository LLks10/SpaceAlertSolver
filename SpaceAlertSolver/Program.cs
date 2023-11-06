using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        Random r = new(seed);
        List<Event> events = new();

        //Setup threat pool
        List<int> comExThreats = new(ThreatFactory.Instance.ExternalCommonThreatIds);
        List<int> sevExThreats = new(ThreatFactory.Instance.ExternalSevereThreatIds);
        List<int> comInThreats = new(ThreatFactory.Instance.InternalCommonThreatIds);
        List<int> sevInThreats = new(ThreatFactory.Instance.InternalSevereThreatIds);

        //Load trajectories
        Trajectory[] trajectoriesArray = new Trajectory[4];
        int[] ts = new int[] { 0, 1, 2, 3, 4, 5, 6 };

        Console.WriteLine("'r' for random trajectories or xxxx to manually set");
        string resp = Console.ReadLine();
        if (resp == "r" || resp == "R")
        {
            for (int i = 0; i < trajectoriesArray.Length; i++)
            {
                int tsi = -1;
                while (tsi == -1)
                    tsi = ts[r.Next(7)];
                trajectoriesArray[i] = new Trajectory(tsi);
                ts[tsi] = -1;
            }
        }
        else
        {
            for (int i = 0; i < trajectoriesArray.Length; i++)
                trajectoriesArray[i] = new Trajectory(int.Parse(resp[i].ToString()) - 1);
        }
        ImmutableArray<Trajectory> trajectories = ImmutableArray.Create(trajectoriesArray);

        Console.WriteLine("Trajectories:");
        Console.WriteLine("Lt{0} Mt{1} Rt{2} It{3}", trajectories[0].number + 1, trajectories[1].number + 1, trajectories[2].number + 1, trajectories[3].number + 1);
        Console.WriteLine();
        Console.WriteLine("Add internal threat: \"<turn> (<name> | r0 | r1)\"");
        Console.WriteLine("Add external threat: \"<turn> <zone (0..2)> (<name> | r0 | r1)\"");
        Console.WriteLine("Start simulation: \"start\"");
        string[] zoneStr = new string[] { "Red", "White", "Blue", "Internal" };
        while (true)
        {
            string str = Console.ReadLine()!;
            if (str == "start")
                break;

            string[] strsplit = str.Split(' ');
            int turn = int.Parse(strsplit[0]);
            if (strsplit.Length >= 3)
            {
                int threatId;
                string threatName;
                if (strsplit[2] == "r0")
                {
                    int i = r.Next(comExThreats.Count);
                    threatId = comExThreats[i];
                    comExThreats.RemoveAt(i);
                    threatName = ThreatFactory.Instance.ThreatNameById[i];
                }
                else if (strsplit[2] == "r1")
                {
                    int i = r.Next(sevExThreats.Count);
                    threatId = sevExThreats[i];
                    sevExThreats.RemoveAt(i);
                    threatName = ThreatFactory.Instance.ThreatNameById[i];
                }
                else
                {
                    (threatName, threatId) = ThreatFactory.Instance.FindThreatMatchingName(strsplit[2]);
                }

                int zone = int.Parse(strsplit[1]);
                events.Add(new(true, turn, zone, threatId));
                Console.WriteLine($"Loaded {threatName} on turn {turn} in zone {zoneStr[zone]} ({threatId})");
            }
            else
            {
                int threatId;
                string threatName;
                if (strsplit[1] == "r0")
                {
                    int i = r.Next(comInThreats.Count);
                    threatId = comInThreats[i];
                    comInThreats.RemoveAt(i);
                    threatName = ThreatFactory.Instance.ThreatNameById[i];
                }
                else if (strsplit[1] == "r1")
                {
                    int i = r.Next(sevInThreats.Count);
                    threatId = sevInThreats[i];
                    sevInThreats.RemoveAt(i);
                    threatName = ThreatFactory.Instance.ThreatNameById[i];
                }
                else
                {
                    (threatName, threatId) = ThreatFactory.Instance.FindThreatMatchingName(strsplit[1]);
                }

                events.Add(new(false, turn, 3, threatId));
                Console.WriteLine($"Loaded {threatName} on turn {turn} in zone {zoneStr[3]} ({threatId})");
            }
        }
        ImmutableArray<Event> evArr = events.OrderBy(e => e.Turn).ToImmutableArray();

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

        var bestGene = sa.getBestGene();
        Task.Run(async () => await UploadToResolver(trajectories, evArr, bestGene)).Wait();
        return bestGene;
    }

    private static async Task UploadToResolver(ImmutableArray<Trajectory> trajectories, ImmutableArray<Event> events, Gene gene)
    {
        var model = gene.ToResolverModel(trajectories, events);

		using var http = new HttpClient();
        var json = JsonSerializer.Serialize(model);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try
        {
			var resp = await http.PostAsync("http://localhost:5000/LoadGame", content);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
	}
}
