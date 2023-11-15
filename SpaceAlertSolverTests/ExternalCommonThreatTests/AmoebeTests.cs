using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests.ExternalCommonThreatTests;

[TestClass]
public sealed class AmoebeTests : ThreatTestsBase
{
    public AmoebeTests() : base("amoebe") { }

    [TestMethod]
    public void HealTest()
    {
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewThreatSpawnStep(_threatId, 1, true));
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewFireCannonStep(Position.TopMiddle, true));
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewProcessDamageStep());

        Assert.AreEqual(5, _defaultGame.Threats[0].MaxHealth - _defaultGame.Threats[0].Health);
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewActXStep(0));
        Assert.AreEqual(3, _defaultGame.Threats[0].MaxHealth - _defaultGame.Threats[0].Health);
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewActYStep(0));
        Assert.AreEqual(1, _defaultGame.Threats[0].MaxHealth - _defaultGame.Threats[0].Health);
    }

    [TestMethod]
    public void OverHealTest()
    {
        Threat threat = _threatPrefab;

        threat.Health--;
        threat.ActX();

        Assert.AreEqual(threat.MaxHealth, threat.Health);

        threat.ActX();

        Assert.AreEqual(threat.MaxHealth, threat.Health);
    }
}
