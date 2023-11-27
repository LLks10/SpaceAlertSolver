using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

[TestClass]
public sealed class ThreatListTests
{
    private static readonly int _pso = TestUtils.GetThreatIdMatchingExactPrimaryName("Power System Overload");
    private static readonly int _amoebe = TestUtils.GetThreatIdMatchingExactPrimaryName("Amoebe");
    private static readonly int _executioner = TestUtils.GetThreatIdMatchingExactPrimaryName("Executioner");
    private static readonly int _armoredGrappler = TestUtils.GetThreatIdMatchingExactPrimaryName("Armored Grappler");

    [TestMethod]
    public void ThreatsStoredCorrectly()
    {
        ThreatList list = GetDefaultList();

        Assert.AreEqual(ThreatFactory.Instance.ThreatsById[_pso], list[0]);
        Assert.AreEqual(ThreatFactory.Instance.ThreatsById[_amoebe], list[1]);
        Assert.AreEqual(ThreatFactory.Instance.ThreatsById[_executioner], list[2]);
        Assert.AreEqual(ThreatFactory.Instance.ThreatsById[_armoredGrappler], list[3]);
    }

    [TestMethod]
    public void TestAllThreatLoop()
    {
        ThreatList list = GetDefaultList();
        
        List<int> result = new();
        foreach (int i in list)
        {
            result.Add(i);
        }

        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(0, result[0]);
        Assert.AreEqual(1, result[1]);
        Assert.AreEqual(2, result[2]);
        Assert.AreEqual(3, result[3]);
    }

    [TestMethod]
    public void TestReverseLoop()
    {
        ThreatList list = GetDefaultList();

        List<int> result = new();
        foreach (int i in list.GetReverseEnumerator())
        {
            result.Add(i);
        }

        Assert.AreEqual(4, result.Count);
        Assert.AreEqual(3, result[0]);
        Assert.AreEqual(2, result[1]);
        Assert.AreEqual(1, result[2]);
        Assert.AreEqual(0, result[3]);
    }

    [TestMethod]
    public void TestInternalLoop()
    {
        ThreatList list = GetDefaultList();
        var internalIds = list.InternalThreatIndices.ToImmutableArray();
        Assert.AreEqual(2, internalIds.Length);
        Assert.AreEqual(0, internalIds[0]);
        Assert.AreEqual(2, internalIds[1]);
    }

    [TestMethod]
    public void TestClear()
    {
        ThreatList list = GetDefaultList();
        list.Clear();
        Assert.AreEqual(0, list.InternalThreatIndices.Count());
        Assert.AreEqual(0, list.ExternalThreatIndices.Count());
        foreach (int i in list)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void TestCopyTo()
    {
        ThreatList list = GetDefaultList();
        ThreatList other = new();
        list.CopyTo(other);

        other.AddThreat(_pso);
        other.AddThreat(_amoebe);

        // adding to other list doesn't change original
        Assert.AreEqual(2, list.InternalThreatIndices.Count());
        Assert.AreEqual(2, list.ExternalThreatIndices.Count());

        // test that other list has correct items
        var internalIds = other.InternalThreatIndices.ToImmutableArray();
        Assert.AreEqual(3, internalIds.Length);
        Assert.AreEqual(0, internalIds[0]);
        Assert.AreEqual(2, internalIds[1]);
        Assert.AreEqual(4, internalIds[2]);

        var externalIds = other.ExternalThreatIndices.ToImmutableArray();
        Assert.AreEqual(3, externalIds.Length);
        Assert.AreEqual(1, externalIds[0]);
        Assert.AreEqual(3, externalIds[1]);
        Assert.AreEqual(5, externalIds[2]);
    }

    private static ThreatList GetDefaultList()
    {
        ThreatList list = new();
        list.AddThreat(_pso);
        list.AddThreat(_amoebe);
        list.AddThreat(_executioner);
        list.AddThreat(_armoredGrappler);
        return list;
    }
}
