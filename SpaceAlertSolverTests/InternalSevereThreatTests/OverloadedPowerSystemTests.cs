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
}
