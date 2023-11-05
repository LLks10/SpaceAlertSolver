using SpaceAlertSolver;

namespace SpaceAlertSolverTests;

[TestClass]
public class PositionTests
{
    [TestMethod]
    public void TestGetLeft()
    {
        Assert.AreEqual(Position.TopLeft, Position.TopLeft.GetLeft());
        Assert.AreEqual(Position.TopLeft, Position.TopMiddle.GetLeft());
        Assert.AreEqual(Position.TopMiddle, Position.TopRight.GetLeft());
        Assert.AreEqual(Position.BottomLeft, Position.BottomLeft.GetLeft());
        Assert.AreEqual(Position.BottomLeft, Position.BottomMiddle.GetLeft());
        Assert.AreEqual(Position.BottomMiddle, Position.BottomRight.GetLeft());
    }

    [TestMethod]
    public void TestGetRight()
    {
        Assert.AreEqual(Position.TopRight, Position.TopRight.GetRight());
        Assert.AreEqual(Position.TopRight, Position.TopMiddle.GetRight());
        Assert.AreEqual(Position.TopMiddle, Position.TopLeft.GetRight());
        Assert.AreEqual(Position.BottomRight, Position.BottomRight.GetRight());
        Assert.AreEqual(Position.BottomRight, Position.BottomMiddle.GetRight());
        Assert.AreEqual(Position.BottomMiddle, Position.BottomLeft.GetRight());
    }

    [TestMethod]
    public void GetElevator()
    {
        Assert.AreEqual(Position.TopLeft, Position.BottomLeft.GetElevator());
        Assert.AreEqual(Position.TopMiddle, Position.BottomMiddle.GetElevator());
        Assert.AreEqual(Position.TopRight, Position.BottomRight.GetElevator());
        Assert.AreEqual(Position.BottomLeft, Position.TopLeft.GetElevator());
        Assert.AreEqual(Position.BottomMiddle, Position.TopMiddle.GetElevator());
        Assert.AreEqual(Position.BottomRight, Position.TopRight.GetElevator());
    }

    [TestMethod]
    public void TestComparison()
    {
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.IsFalse(Position.TopRight.Equals(null));
        Assert.IsTrue(Position.BottomMiddle.Equals((object?)Position.BottomMiddle));
        Assert.IsFalse(Position.Space == Position.BottomRight);
        Assert.IsTrue(Position.TopRight == Position.TopRight);
        Assert.IsTrue(Position.BottomLeft != Position.TopLeft);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [TestMethod]
    public void TestSpace()
    {
        Assert.AreEqual(Position.Space, Position.Space.GetLeft());
        Assert.AreEqual(Position.Space, Position.Space.GetRight());
        Assert.AreEqual(Position.Space, Position.Space.GetElevator());
    }

    [TestMethod]
    public void TestZone()
    {
        Assert.AreEqual(0, Position.BottomLeft.Zone);
        Assert.IsTrue(Position.TopLeft.IsLeft());
        Assert.IsFalse(Position.TopMiddle.IsLeft());
        Assert.AreEqual(1, Position.TopMiddle.Zone);
        Assert.IsTrue(Position.BottomMiddle.IsMiddle());
        Assert.IsTrue(Position.TopRight.IsRight());
        Assert.IsFalse(Position.Space.IsLeft() || Position.Space.IsMiddle() || Position.Space.IsRight());
    }

    [TestMethod]
    public void TestClassifications()
    {
        Assert.IsTrue(Position.TopLeft.IsTop());
        Assert.IsTrue(Position.TopMiddle.IsTop());
        Assert.IsTrue(Position.TopRight.IsTop());
        Assert.IsFalse(Position.BottomLeft.IsTop());
        Assert.IsFalse(Position.BottomMiddle.IsTop());
        Assert.IsFalse(Position.BottomRight.IsTop());
        Assert.IsFalse(Position.Space.IsTop());

        Assert.IsFalse(Position.TopLeft.IsBottom());
        Assert.IsFalse(Position.TopMiddle.IsBottom());
        Assert.IsFalse(Position.TopRight.IsBottom());
        Assert.IsTrue(Position.BottomLeft.IsBottom());
        Assert.IsTrue(Position.BottomMiddle.IsBottom());
        Assert.IsTrue(Position.BottomRight.IsBottom());
        Assert.IsFalse(Position.Space.IsBottom());

        Assert.IsTrue(Position.TopLeft.IsInShip());
        Assert.IsTrue(Position.TopMiddle.IsInShip());
        Assert.IsTrue(Position.TopRight.IsInShip());
        Assert.IsTrue(Position.BottomLeft.IsInShip());
        Assert.IsTrue(Position.BottomMiddle.IsInShip());
        Assert.IsTrue(Position.BottomRight.IsInShip());
        Assert.IsFalse(Position.Space.IsInShip());
    }
}
