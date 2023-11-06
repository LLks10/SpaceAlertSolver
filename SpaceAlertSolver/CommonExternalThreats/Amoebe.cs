namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ThreatId(1)]
    [PrimaryThreatName("Amoebe")]
    public static Threat CreateAmoebe()
    {
        return new Threat(AmoebeHeal, AmoebeHeal, AmoebeActZ,
            8, 0, 2, 4, 2, rocketImmune: true) { MaxHealth = 8 };
    }

    private static void AmoebeHeal(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 2, @this.MaxHealth);
    }

    private static void AmoebeActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 4);
    }
}
