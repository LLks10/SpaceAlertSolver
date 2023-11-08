namespace SpaceAlertSolver;

internal partial struct Threat
{
    private int MaxHealth { readonly get => _value1; set => _value1 = value; }

    [ExternalCommonThreat("E1-08", "Armored Grappler", "Gepanserde Grijper")]
    public static Threat CreateArmoredGrappler()
    {
        return new Threat(4, 3, 2, 4, 2) { MaxHealth = 4 };
    }

    public static void ArmoredGrapplerActX(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 1);
    }

    public static void ArmoredGrapplerActY(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 1, @this.MaxHealth);
    }

    public static void ArmoredGrapplerActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 4);
    }
}
