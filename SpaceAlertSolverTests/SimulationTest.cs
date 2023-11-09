using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public class SimulationTest
{
    [TestMethod]
    public void Test3pBlank()
    {
        Player emptyPlayer = TestUtils.CreatePlayerFromActions(ActUtils.ParseActionsFromString("            "));
        Player[] players = new Player[3];
        Array.Fill(players, emptyPlayer);

        ImmutableArray<Trajectory> trajectories = TestUtils.GetTrajectoriesFromString("1234");
        ImmutableArray<Event> events = ImmutableArray<Event>.Empty;

        Game g = GamePool.GetGame();
        g.Init(players, trajectories, events);
        double score = g.Simulate();
        GamePool.FreeGame(g);
        Assert.AreEqual(0.0, score);
    }

    [TestMethod]
    public void Test1Enemy0Damage()
    {
        Act[] actions = ActUtils.ParseActionsFromString("a           ");
        Player[] players = new Player[1] { TestUtils.CreatePlayerFromActions(actions) };
        ImmutableArray<Trajectory> trajectories = TestUtils.GetTrajectoriesFromString("1234");
        ImmutableArray<Event> events = ImmutableArray.Create
        (
            new Event(true, 1, 1, TestUtils.GetThreatIdMatchingExactPrimaryName("Meteoroid")) // meteorite will be oneshot
        );

        Game g = GamePool.GetGame();
        g.Init(players, trajectories, events);
        double score = g.Simulate();
        GamePool.FreeGame(g);
        Assert.AreEqual(4.0, score);
    }

    [TestMethod]
    public void TestSimpleBattleship1Damage()
    {
        // here we put 1 battleship
        // wait for it to deal 2 damage (1 damage past shield) (shoots end t2)
        // then shoot it twice (t3, t4), a broken cannon will just not kill it
        // score if cannon not broken = 2.0 with 5/6% chance
        // score if cannon is broken = -6.0 with 1/6% chance
        // expected score = 2/3

        Act[] actions = ActUtils.ParseActionsFromString("c aa        ");
        Player[] players = new Player[1] { TestUtils.CreatePlayerFromActions(actions) };
        ImmutableArray<Trajectory> trajectories = TestUtils.GetTrajectoriesFromString("1234");
        ImmutableArray<Event> events = ImmutableArray.Create
        (
            new Event(true, 1, 1, TestUtils.GetThreatIdMatchingExactPrimaryName("Gunship"))
        );

        Game g = GamePool.GetGame();
        g.Init(players, trajectories, events);
        double score = g.Simulate();
        GamePool.FreeGame(g);
        Assert.AreEqual(2.0 / 3.0, score, 0.000000001);
    }

    [TestMethod]
    public void TestNemesisShieldBranch()
    {
        // we put nemesis and one other threat
        // break nemesis shield 2/3 times to damage all zones
        // branch for broken shield which results in survival or loss in other zone
        // score shield not broken = 0.0 with 5/6% chance
        // score shield is broken = -209.0 with 1/6% chance

        Act[] actions0 = ActUtils.ParseActionsFromString("aa          ");
        Act[] actions1 = ActUtils.ParseActionsFromString("da          ");
        Act[] actions2 = ActUtils.ParseActionsFromString("crb         ");

        Player[] players = TestUtils.CreatePlayersFromActions(actions0, actions1, actions2);
        ImmutableArray<Trajectory> trajectories = TestUtils.GetTrajectoriesFromString("6134");
        ImmutableArray<Event> events = ImmutableArray.Create
        (
            new Event(true, 1, 1, TestUtils.GetThreatIdMatchingExactPrimaryName("Nemesis")),
            new Event(true, 2, 2, TestUtils.GetThreatIdMatchingExactPrimaryName("Gunship"))
        );

        Game g = GamePool.GetGame();
        g.Init(players, trajectories, events);
        double score = g.Simulate();
        GamePool.FreeGame(g);
        Assert.AreEqual(-209.0 / 6.0, score, 0.000000001);
    }
}
