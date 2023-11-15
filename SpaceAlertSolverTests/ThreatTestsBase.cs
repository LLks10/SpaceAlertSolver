using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

public class ThreatTestsBase
{
    protected readonly int _threatId;
    protected readonly string _threatName;
    protected readonly Threat _threatPrefab;

    /// <summary>
    /// This game has no players or events and is freshly created for each test
    /// </summary>
    protected Game _defaultGame = null!;

    protected ThreatTestsBase(string threatName)
    {
        var (_, i) = ThreatFactory.Instance.FindThreatMatchingName(threatName);
        _threatId = i;
        _threatName = ThreatFactory.Instance.ThreatNameById[i];
        _threatPrefab = ThreatFactory.Instance.ThreatsById[i];
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _defaultGame = GamePool.GetGame();
        _defaultGame.Init(Array.Empty<Player>(), TestUtils.GetTrajectoriesFromString("1234"), simulationStack: new());
    }

    [TestCleanup]
    public void TestCleanup()
    {
        GamePool.FreeGame(_defaultGame);
    }
}
