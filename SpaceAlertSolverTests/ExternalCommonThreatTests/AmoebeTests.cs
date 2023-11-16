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
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewThreatSpawnStep(_threatId, 1));
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

    [TestMethod]
    public void RocketImmuneTest()
    {
        _defaultGame.Init(TestUtils.CreatePlayersFromActions(ActUtils.ParseActionsFromString("c           ")), _defaultGame.trajectories, simulationStack: new());
        var (_, gunshipId) = ThreatFactory.Instance.FindThreatMatchingName("gunship");
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewThreatSpawnStep(_threatId, 1));
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewThreatSpawnStep(gunshipId, 1));
        _defaultGame.Threats[0].Distance = 5;
        _defaultGame.Threats[1].Distance = 5;
        _defaultGame.Players[0].TryMoveRight();
        _defaultGame.Players[0].TryTakeElevator();

        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewRocketUpdateStep());
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewRocketUpdateStep());
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewProcessDamageStep());

        Assert.AreEqual(_defaultGame.Threats[0].MaxHealth, _defaultGame.Threats[0].Health);
        Assert.AreEqual(_defaultGame.Threats[1].MaxHealth - 3 + _defaultGame.Threats[1].Shield, _defaultGame.Threats[1].Health);
    }
}
