using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public class FullSimulationTests
{
    [TestMethod]
    public void TestStandardMission()
    {
        /* 7643
         * 1 2 amoebe
         * 3 0 psionische satelliet
         * 4 3 exterminator
         * 5 1 cryoschild jager
         * 7 0 impulsbal
         * 8 1 verkenner
         * start
         * 
         *       1     2     3     4     5     6     7     8     9    10    11    12
         * P |  Lift   C     B    Blue   C     C    Blue   -     C    Red    -     C
         * R |   B   Robot   -     C    Red    A     A     -    Lift   -    Blue   C
         * Y |   C    Blue   A     A     -     -    Red    B    Lift   -     -     C
         * G |  Blue   -     C   Robot   C   Robot  Red  Robot   A    Lift   C     -
        */

        ImmutableArray<Trajectory> trajectories = TestUtils.GetTrajectoriesFromString("7643");
        ImmutableArray<Event> events = ImmutableArray.Create<Event>
        (
            "1 2 amoebe",
            "3 0 psionic satellite",
            "4 executioner",
            "5 1 cryoshield fighter",
            "7 0 pulse ball",
            "8 1 scout"
        );

        Act[][] actions = new Act[][]
            { ActUtils.ParseActionsFromString("dcbrccr ce c")
            , ActUtils.ParseActionsFromString("bf ceaa d rc")
            , ActUtils.ParseActionsFromString("craa  ebd  c")
            , ActUtils.ParseActionsFromString("r cfcfefadc ")
            };

        Game game = GamePool.GetGame();
        game.Init(actions.Select(a => new Player(ImmutableArray.Create(a))).ToArray(), trajectories, events);
        double score = game.Simulate();
        GamePool.FreeGame(game);
        
        Assert.AreEqual(41.0, score);
    }

    [TestMethod]
    public void TestBenchmark()
    {
        ImmutableArray<Trajectory> trajectories = TestUtils.GetTrajectoriesFromString("1463");
        ImmutableArray<Event> events = ImmutableArray.Create<Event>
        (
            "1 1 cryoshield frigate",
            "3 0 kamikaze",
            "4 power system overload",
            "6 1 scout"
        );
        Player[] players = TestUtils.CreatePlayersFromActions
        (
            ActUtils.ParseActionsFromString(" ardacca eaa"),
            ActUtils.ParseActionsFromString("dbc  cdbd df"),
            ActUtils.ParseActionsFromString("bcabafcreaeb"),
            ActUtils.ParseActionsFromString("bfeaaedfrbc ")
        );

        Game g = GamePool.GetGame();
        g.Init(players, trajectories, events);
        Assert.AreEqual(16.5, g.Simulate());
        GamePool.FreeGame(g);

        Assert.AreEqual(4, Game.Scores.Count);
        Assert.IsTrue(Game.Scores.All(s => s == 16 || s == 17));
    }
}
