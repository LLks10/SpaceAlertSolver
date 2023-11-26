using SpaceAlertSolver;

namespace SpaceAlertSolverTests.InternalSevereThreatTests;

[TestClass]
public sealed class PowerSystemOverload : ThreatTestsBase
{
    public PowerSystemOverload() : base("Power System Overload") { }

    [TestMethod]
    public void PlayersKillExactlyTest()
    {
        Threat threat = _threatPrefab;

        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomLeft);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomLeft);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomLeft);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomMiddle);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomRight);
        
        Assert.IsFalse(threat.Alive);
    }

    [TestMethod]
    public void PlayersAreOneDamageShort()
    {
        Threat threat = _threatPrefab;

        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomLeft);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomLeft);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomMiddle);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomRight);

        Assert.IsTrue(threat.Alive);
    }

    [TestMethod]
    public void BonusDamageNotInSameTurn()
    {
        Threat threat = _threatPrefab;

        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomLeft);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomMiddle);
        threat.ProcessDamageOrEndTurn();
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomRight);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomRight);
        threat.DealInternalDamage(DamageSource.RepairB, 1, -1, Position.BottomRight);

        Assert.IsTrue(threat.Alive);
    }

    [TestMethod]
    public void MalfunctionTest()
    {
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewThreatSpawnStep(_threatId, 3));
        Assert.IsTrue(_defaultGame.ship.HasMalfunctionB(Position.BottomLeft));
        Assert.IsTrue(_defaultGame.ship.HasMalfunctionB(Position.BottomMiddle));
        Assert.IsTrue(_defaultGame.ship.HasMalfunctionB(Position.BottomRight));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.TopLeft));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.TopMiddle));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.TopRight));

        _defaultGame.Threats[0].Alive = false;
        _defaultGame.Threats[0].Beaten = true;
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewCleanExternalThreatsStep());

        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.BottomLeft));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.BottomMiddle));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.BottomRight));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.TopLeft));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.TopMiddle));
        Assert.IsFalse(_defaultGame.ship.HasMalfunctionB(Position.TopRight));
    }

    [TestMethod]
    public void CanRefillInTurnOfKill()
    {
        _defaultGame.Init(TestUtils.CreatePlayersFromActions(ActUtils.ParseActionsFromString("bbbbbbbbbbbb")), _defaultGame.trajectories, simulationStack: new());
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewThreatSpawnStep(_threatId, 3));

        // hit middle
        _defaultGame.Players[0].TryTakeElevator();
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));

        // hit left
        _defaultGame.Players[0].TryMoveLeft();
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));

        // hit right & kill
        _defaultGame.Players[0].TryMoveRight();
        _defaultGame.Players[0].TryMoveRight();
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));

        // refill main reactor
        _defaultGame.Players[0].TryMoveLeft();
        _defaultGame.PerformSingleSimulationStep(SimulationStep.NewPlayerActionStep(0));

        Assert.AreEqual(5, _defaultGame.ship.Reactors[1]);
    }
}