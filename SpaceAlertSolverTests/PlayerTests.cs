using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void TestMovement()
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
}
