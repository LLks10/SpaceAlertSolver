namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ExternalCommonThreat("Amoebe")]
    public static Threat CreateAmoebe()
    {
        return new Threat(8, 0, 2, 4, 2, rocketImmune: true, actX: AmoebeHeal, actY: AmoebeHeal) { MaxHealth = 8 };
    }

    public static void AmoebeHeal(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 2, @this.MaxHealth);
    }

    public static void AmoebeActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 4);
    }
}
