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

        ImmutableArray<Trajectory> trajectories = TrajectoryUtils.GetTrajectoriesFromString("7643");
        ImmutableArray<Event> events = ImmutableArray.Create<Event>
            ( new(true, 1, 2, ThreatParser.ParseExThreat("amoebe").id)
            , new(true, 3, 0, ThreatParser.ParseExThreat("psionische satelliet").id)
            , new(false, 4, 3, ThreatParser.ParseInThreat("elminator").id)
            , new(true, 5, 1, ThreatParser.ParseExThreat("cryoschild jager").id)
            , new(true, 7, 0, ThreatParser.ParseExThreat("impulsbal").id)
            , new(true, 8, 1, ThreatParser.ParseExThreat("verkenner").id)
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
}
