namespace SpaceAlertSolver;

public partial struct Threat
{
    private bool HasShield { readonly get => _value1 > 0; set => _value1 = value ? 1 : 0; }

    [ExternalCommonThreat("E1-06", "Cryoshield Fighter", "Cryoschild Jager")]
    public static Threat CreateCryoshieldFighter()
    {
        return new Threat(4, 1, 3, 4, 2, actX: ActDealDamage(1), actY: ActDealDamage(2), actZ: ActDealDamage(2), processDamage: CryoshieldProcessDamage) { HasShield = true };
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
