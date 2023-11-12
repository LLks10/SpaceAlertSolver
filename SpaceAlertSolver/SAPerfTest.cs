using System.Collections.Immutable;

namespace SpaceAlertSolver;

internal static class SAPerfTest
{
	static int[] iterations = new int[] { 10_000, 100_000, 1_000_000, 5_000_000 };
	static int[] seeds = new int[] { 123, 456, 789 };
	static GameSetup[] games = new GameSetup[]
	{
		new GameSetup("Mis2_UnCon4p", 4, "4512", "1,2,10;2,3,11;3,1,15;4,2,3;5,1,14;6,3,23;8,0,12"),
		new GameSetup("Mis4_Unlucky", 5, "7542", "1,0,15;3,1,13;4,0,27;5,3,18;6,2,28"),
		new GameSetup("Mis7_UnCon4p", 4, "7312", "1,2,0;3,0,22;4,3,20;5,1,13;7,0,9;8,1,2"),
		new GameSetup("12Common_4p" , 4, "1764", "1,0,1;2,1,12;3,2,6;4,3,9;5,1,2;6,0,5;7,3,7;8,2,13;9,1,7;10,3,6;11,0,0;12,2,4"),
		new GameSetup("Terror_5p"   , 5, "5614", "1,0,21;2,1,12;3,2,18;4,3,6;5,1,22;6,0,13;7,3,14;8,2,5;9,1,2;10,3,1;11,0,0;12,2,11"),
		new GameSetup("Terror_6p"   , 6, "3142", "1,0,26;2,1,25;3,0,9;4,3,9;5,2,21;6,1,17;7,3,19;8,2,13;9,0,22;10,3,20;11,1,23;12,2,20"),
	};

	public static void Run()
	{
		List<(double Score, double Timing)> scores = new();

		foreach(int iter in iterations)
		foreach(GameSetup setup in games)
		foreach(int seed in seeds)
		{
			Console.WriteLine($"Running {setup.name} at {iter} iterations (Seed: {seed})");
			var sa = new SimulatedAnnealing(setup.playerCount, setup.trajectory, setup.events);
			sa.Run(iter, setup.trajectory, setup.events, seed, false);
			var bestScore = sa.BestGeneAverage;
			var timing = sa.Timing;
			scores.Add((bestScore, timing));
			Console.WriteLine(sa.getBestGene().Rep());
			Program.PrintStats(sa.getBestStats());
			Console.WriteLine("\n=================================\n");
		}

		Console.WriteLine("Final scores");
		foreach (var (score, timing) in scores)
			Console.WriteLine($"{score}\t\t{timing}");
	}

	private struct GameSetup
	{
		public string name;
		public int playerCount;
		public ImmutableArray<Trajectory> trajectory;
		public ImmutableArray<Event> events;

		public GameSetup(string name, int players, string trajectories, string events)
		{
			this.name = name;
			playerCount = players;
			var t = new Trajectory[4];
			for (int i = 0; i < t.Length; i++)
				t[i] = new Trajectory(int.Parse(trajectories[i].ToString()) - 1);
			trajectory = t.ToImmutableArray();

			var evs = events.Split(';');
			var e = new List<Event>();
			foreach (var ev in evs)
			{
				var splt = ev.Split(',');
				var z = int.Parse(splt[1]);
				e.Add(new Event(z < 3, int.Parse(splt[0]), z, int.Parse(splt[2])));
			}
			this.events = e.ToImmutableArray();
		}
	}
}
