using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void MovementTest()
    {
        Player player = new(ImmutableArray<Act>.Empty);
        player.TryMoveLeft();
        player.TryMoveLeft();
        player.TryTakeElevator();
        player.TryMoveLeft();
        player.TryMoveRight();
        player.TryMoveRight();
        Assert.AreEqual(Position.BottomRight, player.Position);
    }

    [TestMethod]
    public void DelayTest()
    {
        Act[] actions = ActUtils.ParseActionsFromString("a bca  b cab");
        Player player = TestUtils.CreatePlayerFromActions(actions);

        Assert.AreEqual(Act.A, player.GetNextAction());
        player.DelayNext(); // a-bca--b-cab (no change)
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        player.DelayNext(); // a--bca-b-cab
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        player.DelayNext(); // a---bcab-cab
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        Assert.AreEqual(Act.B, player.GetNextAction());
        Assert.AreEqual(Act.C, player.GetNextAction());
        player.DelayNext(); // a---bc-abcab
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        Assert.AreEqual(Act.A, player.GetNextAction());
        player.DelayNext(); // a---bc-a-bca
        player.DelayNext();
        player.DelayNext();
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        Assert.AreEqual(Act.B, player.GetNextAction());
        Assert.AreEqual(Act.C, player.GetNextAction());
        player.DelayNext(); // a---bc-a-bc-
        Assert.AreEqual(Act.Empty, player.GetNextAction());
    }

    [TestMethod]
    public void DelayCurrentTest()
    {
        Act[] actions = new Act[] { Act.Empty, Act.Fight, Act.B, Act.Empty };
        Player player = TestUtils.CreatePlayerFromActions(actions);

        Assert.AreEqual(Act.Empty, player.GetNextAction());
        Assert.AreEqual(Act.Fight, player.GetNextAction());
        player.DelayCurrent();
        Assert.AreEqual(Act.Fight, player.GetNextAction());
        Assert.AreEqual(Act.B, player.GetNextAction());
    }

    [TestMethod]
    public void DelayCurrentEmptyTest()
    {
        Act[] actions = new Act[] { Act.Left, Act.Empty, Act.Right };
        Player player = TestUtils.CreatePlayerFromActions(actions);

        Assert.AreEqual(Act.Left, player.GetNextAction());
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        player.DelayCurrent();
        Assert.AreEqual(Act.Right, player.GetNextAction());
    }

    [TestMethod]
    public void DelayCurrentWhenDelayedTest()
    {
        Act[] actions = new Act[] { Act.Lift, Act.Left, Act.Right, Act.A, Act.B, Act.C, Act.Fight };
        Player player = TestUtils.CreatePlayerFromActions(actions);

        player.DelayNext(); // -dlrabc
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        player.DelayCurrent(); // -dlrabc
        Assert.AreEqual(Act.Lift, player.GetNextAction());

        player.DelayNext(); // -d-lrab
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        player.DelayNext(); // -d--lra
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        Assert.AreEqual(Act.Left, player.GetNextAction());
        player.DelayCurrent(); // -d---lr
        Assert.AreEqual(Act.Left, player.GetNextAction());
    }

    [TestMethod]
    public void DelayCurrentThenNextTest()
    {
        Act[] actions = new Act[] { Act.Lift, Act.Right, Act.A, Act.Fight, Act.C, Act.B };
        Player player = TestUtils.CreatePlayerFromActions(actions);

        player.GetNextAction();
        player.DelayCurrent(); // -drafc
        player.DelayNext();    // --draf
        Assert.AreEqual(Act.Empty, player.GetNextAction());
        Assert.AreEqual(Act.Lift, player.GetNextAction());
    }

    [TestMethod]
    public void DelayNextThenCurrentTest()
    {
        Act[] actions = new Act[] { Act.Lift, Act.Right, Act.A, Act.Fight, Act.C, Act.B };
        Player player = TestUtils.CreatePlayerFromActions(actions);

        player.GetNextAction();
        player.DelayNext(); // d-rafc
        player.DelayCurrent(); // -drafc
        Assert.Equals(Act.Lift, player.GetNextAction());
        Assert.Equals(Act.Right, player.GetNextAction());
    }
}
