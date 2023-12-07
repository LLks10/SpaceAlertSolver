using SimulationGenerator;
using SpaceAlertSolver;
using SpaceAlertSolver.ResolverInterop;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public class BulkTests
{
    [TestMethod]
    public void BulkTest()
    {
        string filePath = Path.Combine(TestStatics.FolderPath, "simulations.txt");
        Simulation[] simulations = ReadSimulationsFromFile(filePath);
        BatchResolver externalResolver = new();
        List<List<int>> selfResults = new();
        for (int i = 0; i < simulations.Length; i++)
        {
            int numPlayers = simulations[i].Actions.Length;
            int[] geneArray = new int[12 * numPlayers];
            for (int j = 0; j < numPlayers; j++)
            {
                for (int k = 0; k < 12; k++)
                {
                    geneArray[j * 12 + k] = (int)simulations[i].Actions[j][k];
                }
            }
            Gene gene = new(geneArray, numPlayers);
            ImmutableArray<Trajectory> trajectories = TrajectoryUtils.GetTrajectoriesFromString(simulations[i].Trajectories);
            ImmutableArray<Event> events = simulations[i].EventStrings
                .Select(e =>
                {
                    string[] parts = e.Split(' ');
                    int turn = int.Parse(parts[0]);
                    int zone = int.Parse(parts[1]);
                    bool isExternal = zone < 3;
                    string threatName = String.Join(' ', parts[2..]);
                    int threatId;
                    if (isExternal)
                        threatId = ThreatParser.ParseExThreat(threatName).id;
                    else
                        threatId = ThreatParser.ParseInThreat(threatName).id;
                    return new Event(isExternal, turn, zone, threatId);
                })
                .ToImmutableArray();
            
            externalResolver.AddGame(gene, trajectories, events);
            Game game = GamePool.GetGame();
            game.Init(TestUtils.CreatePlayersFromActions(simulations[i].Actions), trajectories, events);
            Game.Scores.Clear();
            game.Simulate();
            GamePool.FreeGame(game);
            selfResults.Add(new(Game.Scores));
        }

        List<ResolverResult> externalResults = externalResolver.BatchResolve();
        Assert.AreEqual(selfResults.Count, externalResults.Count);
        for (int i = 0; i < selfResults.Count; i++)
        {
            bool matchesOne = false;
            foreach (int selfScore in selfResults[i])
            {
                bool won = selfScore > -100;
                if (won != externalResults[i].Won)
                    continue;

                if (!won)
                {
                    matchesOne = true;
                    break;
                }
                if (selfScore == externalResults[i].Score)
                {
                    matchesOne = true;
                    break;
                }
            }
            if (!matchesOne)
            {
                Assert.Fail($"Scores do not match for game {i}.\r\n" +
                    $"String to create the game is\r\n{simulations[i].Serialize()}\r\n\r\n" +
                    $"Result from external resolver is won: {externalResults[i].Won}, score: {externalResults[i].Score}\r\n" +
                    $"Possible scores from internal resolver are: {String.Join(' ', selfResults[i])}");
            }
        }
    }

    private Simulation[] ReadSimulationsFromFile(string file)
    {
        string data;
        using (FileStream fs = new(file, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new(fs))
            {
                data = sr.ReadToEnd();
            }
        }

        string[] simulationStrings = data.Split(Environment.NewLine + Environment.NewLine);
        Simulation[] simulations = simulationStrings.Where(s => s.Length > 0).Select(Simulation.Deserialize).ToArray();
        return simulations;
    }
}
