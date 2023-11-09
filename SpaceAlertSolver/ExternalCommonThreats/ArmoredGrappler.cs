namespace SpaceAlertSolver;

internal partial struct Threat
{
    private int MaxHealth { readonly get => _value1; set => _value1 = value; }

    [ExternalCommonThreat("E1-08", "Armored Grappler", "Gepantserde Grijper")]
    public static Threat CreateArmoredGrappler()
    {
        return new Threat(4, 3, 2, 4, 2, actX: ActDealDamage(1), actZ: ActDealDamage(4)) { MaxHealth = 4 };
    }

    public static void ArmoredGrapplerActY(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 1, @this.MaxHealth);
    }
}
