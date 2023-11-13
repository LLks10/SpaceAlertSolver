using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    private int DamageBitmask
    {
        get => _value1;
        set => _value1 = value;
    }
    private const int FULL_BOTTOM_DAMAGE_BITMASK = (1 << Position.BOTTOM_LEFT_INDEX) | (1 << Position.BOTTOM_MIDDLE_INDEX) | (1 << Position.BOTTOM_RIGHT_INDEX);

    [InternalSevereThreat("SI2-03", "Power System Overload", "Overbelast Energienet")]
    public static Threat CreatePowerSystemOverload()
    {
        return new Threat(7, 0, Position.BottomLeft, 12, 6);
    }

    public static bool PowerSystemOverloadIsTargetedBy(ref Threat @this, DamageSource damageSource, Position position)
    {
        return damageSource == DamageSource.RepairB && position.IsBottom();
    }

    public static void PowerSystemOverloadOnSpawn(ref Threat @this)
    {
        @this.Game.AddMalfunctionB(Position.BottomLeft);
        @this.Game.AddMalfunctionB(Position.BottomMiddle);
        @this.Game.AddMalfunctionB(Position.BottomRight);
    }

    public static void PowerSystemOverloadOnBeaten(ref Threat @this)
    {
        if (!@this.Alive)
        {
            @this.Game.RemoveMalfunctionB(Position.BottomLeft);
            @this.Game.RemoveMalfunctionB(Position.BottomMiddle);
            @this.Game.RemoveMalfunctionB(Position.BottomRight);
        }
    }

    public static void PowerSystemOverloadDealDamage(ref Threat @this, DamageSource damageSource, int damage, int _, Position position)
    {
        Debug.Assert(damageSource == DamageSource.RepairB);
        Debug.Assert(position.IsBottom());
        @this.Health -= damage;
        @this.DamageBitmask |= position.PositionIndex;
        if (@this.DamageBitmask == FULL_BOTTOM_DAMAGE_BITMASK)
        {
            @this.Health -= 2;
            @this.DamageBitmask = -1; // block this from triggering again until turn end
        }

        @this.UpdateAlive();
    }

    public static void PowerSystemOverloadProcessDamage(ref Threat @this)
    {
        @this.DamageBitmask = 0;
    }

    public static void PowerSystemOverloadActX(ref Threat @this)
    {
        @this.Game.SpillEnergy(Position.BottomMiddle, 2);
    }

    public static void PowerSystemOverloadActY(ref Threat @this)
    {
        @this.Game.SpillEnergy(Position.BottomLeft, 1);
        @this.Game.SpillEnergy(Position.BottomMiddle, 1);
        @this.Game.SpillEnergy(Position.BottomRight, 1);
    }

    public static void PowerSystemOverloadActZ(ref Threat @this)
    {
        @this.Game.DealInternalDamage(0, 3);
        @this.Game.DealInternalDamage(1, 3);
        @this.Game.DealInternalDamage(2, 3);
    }
}
