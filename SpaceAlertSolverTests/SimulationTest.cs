using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public class SimulationTest
{
    [TestMethod]
    public void Test3pBlank()
    {
        Player[] players = new Player[3]{ GetEmptyPlayer(), GetEmptyPlayer(), GetEmptyPlayer() };
        Trajectory[] trajectories = ConstructTrajectories(1, 2, 3, 4);
        Event[] events = new Event[]{ };

        Game g = new Game(players, trajectories, events);
        double score = g.Simulate();
        Assert.AreEqual(0.0, score);
    }

    [TestMethod]
    public void Test1Enemy0Damage()
    {
        Act[] actions = GetEmptyActions();
        actions[0] = Act.A;
        Player[] players = new Player[1] { new(ImmutableArray.Create(actions)) };
        Trajectory[] trajectories = ConstructTrajectories(1, 2, 3, 4);
        Event[] events = new Event[]
        {
            new Event(true, 1, 1, 6) // meteorite will be oneshot
        };

        Game g = new Game(players, trajectories, events);
        double score = g.Simulate();
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

        Act[] actions = GetEmptyActions();
        actions[0] = Act.C;
        actions[2] = Act.A;
        actions[3] = Act.A;
        Player[] players = new Player[1] { new(ImmutableArray.Create(actions)) };
        Trajectory[] trajectories = ConstructTrajectories(1, 2, 3, 4);
        Event[] events = new Event[]
        {
            new Event(true, 1, 1, 2)
        };

        Game g = new Game(players, trajectories, events);
        double score = g.Simulate();
        Assert.AreEqual(2.0 / 3.0, score, 0.000000001);
    }

    [TestMethod]
    public void TestNemesisShieldBranch()
    {
        // we put nemesis and one other threat
        // break nemesis shield 2/3 times to damage all zones
        // branch for broken shield which results in survival or loss in other zone
        // score shield not broken = 0.0 with 5/6% chance
        // score shield is broken = -194.0 with 1/6% chance

        Act[] actions0 = GetEmptyActions();
        Act[] actions1 = GetEmptyActions();
        Act[] actions2 = GetEmptyActions();

        actions0[0] = Act.A;
        actions0[1] = Act.A;
        actions1[0] = Act.Lift;
        actions1[1] = Act.A;
        actions2[0] = Act.C;
        actions2[1] = Act.Right;
        actions2[2] = Act.B;

        Player[] players = new Player[3] { new(ImmutableArray.Create(actions0)), new(ImmutableArray.Create(actions1)), new(ImmutableArray.Create(actions2)) };
        Trajectory[] trajectories = ConstructTrajectories(6, 1, 3, 4);
        Event[] events = new Event[]
        {
            new Event(true, 1, 1, 23),
            new Event(true, 2, 2, 2)
        };

        Game g = new Game(players, trajectories, events);
        double score = g.Simulate();
        Assert.AreEqual(-194.0 / 6.0, score, 0.000000001);
    }

    private Player GetEmptyPlayer()
    {
        return new Player(ImmutableArray.Create(GetEmptyActions()));
    }

    private Act[] GetEmptyActions()
    {
        Act[] output = new Act[12];
        Array.Fill(output, Act.Empty);
        return output;
    }

    private Trajectory[] ConstructTrajectories(int l, int m, int r, int i)
    {
        return new Trajectory[4]
        {
            new Trajectory(l-1),
            new Trajectory(m-1),
            new Trajectory(r-1),
            new Trajectory(i-1)
        };
    }
}
