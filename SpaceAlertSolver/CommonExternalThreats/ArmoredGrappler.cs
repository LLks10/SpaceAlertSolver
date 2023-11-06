namespace SpaceAlertSolver;

internal partial struct Threat
{
    private int MaxHealth { readonly get => _value1; set => _value1 = value; }

    [ThreatId(0)]
    [PrimaryThreatName("Armored Grappler")]
    [ThreatName("Gepanserde Grijper")]
    public static Threat CreateArmoredGrappler()
    {
        return new Threat(ArmoredGrapplerActX, ArmoredGrapplerActY, ArmoredGrapplerActZ,
            4, 3, 2, 4, 2) { MaxHealth = 4 };
    }

    private static void ArmoredGrapplerActX(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 1);
    }

    private static void ArmoredGrapplerActY(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 1, @this.MaxHealth);
    }

    private static void ArmoredGrapplerActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 4);
    }
}
