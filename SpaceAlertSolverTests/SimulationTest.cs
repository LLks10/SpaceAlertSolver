using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpaceAlertSolver;

namespace SpaceAlertSolverTests
{
    [TestClass]
    public class SimulationTest
    {
        [TestMethod]
        public void Test3pBlank()
        {
            Player[] players = new Player[3]{ GetEmptyPlayer(), GetEmptyPlayer(), GetEmptyPlayer() };
            Trajectory[] trajectories = ConstructTrajectories(0, 1, 2, 3);
            Event[] events = new Event[]{ };

            Game g = new Game(players, trajectories, events);
            double score = g.Simulate();
            Assert.AreEqual(0.0, score);
        }

        [TestMethod]
        public void Test1p1Enemy0Damage()
        {
            Player[] players = new Player[1] { GetEmptyPlayer() };
            players[0].actions[0] = Act.A;
            Trajectory[] trajectories = ConstructTrajectories(0, 1, 2, 3);
            Event[] events = new Event[]
            {
                new Event(true, 1, 1, 6) // meteorite will be oneshot
            };

            Game g = new Game(players, trajectories, events);
            double score = g.Simulate();
            Assert.AreEqual(4.0, score);
        }

        [TestMethod]
        public void Test1pBattleship1Damage()
        {
            // here we put 1 battleship
            // wait for it to deal 2 damage (1 damage past shield) (shoots end t2)
            // then shoot it twice (t3, t4), a broken cannon will just not kill it
            // score if cannon not broken = 2.0 with 5/6% chance
            // score if cannon is broken = -6.0 with 1/6% chance
            // expected score = 2/3

            Player[] players = new Player[1] { GetEmptyPlayer() };
            players[0].actions[0] = Act.C;
            players[0].actions[2] = Act.A;
            players[0].actions[3] = Act.A;
            Trajectory[] trajectories = ConstructTrajectories(0, 1, 2, 3);
            Event[] events = new Event[]
            {
                new Event(true, 1, 1, 2)
            };

            Game g = new Game(players, trajectories, events);
            double score = g.Simulate();
            Assert.AreEqual(2.0 / 3.0, score, 0.000000001);
        }

        private Player GetEmptyPlayer()
        {
            return new Player(new Act[12] {
                Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty, Act.empty
            });
        }

        private Trajectory[] ConstructTrajectories(int l, int m, int r, int i)
        {
            return new Trajectory[4]
            {
                new Trajectory(l),
                new Trajectory(m),
                new Trajectory(r),
                new Trajectory(i)
            };
        }
    }
}
