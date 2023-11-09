namespace SpaceAlertSolver;

internal partial struct Threat
{
    private bool HasShield { readonly get => _value1 > 0; set => _value1 = value ? 1 : 0; }

    [ExternalCommonThreat("E1-06", "Cryoshield Fighter", "Cryoschild Jager")]
    public static Threat CreateCryoshieldFighter()
    {
        return new Threat(4, 1, 3, 4, 2, actY: CryoshieldFighterActYZ, actZ: CryoshieldFighterActYZ, processDamage: CryoshieldProcessDamage) { HasShield = true };
    }

    public static void CryoshieldFighterActX(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 1);
    }

    private static void CryoshieldFighterActYZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 2);
    }

    private static void CryoshieldProcessDamage(ref Threat @this)
    {
        if (@this.HasShield && @this.Damage > 0)
        {
            @this.HasShield = false;
            @this.Damage = 0;
            return;
        }
        DefaultExternalProcessDamage(ref @this);
    }
}
