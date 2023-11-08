namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ExternalCommonThreat("E1-10", "Meteoroid", "Meteoroïde")]
    public static Threat CreateMeteoroid()
    {
        return new Threat(5, 0, 5, 4, 2, actX: Blank, actY: Blank, getDistance: RocketImmuneGetDistance);
    }

    public static void MeteoroidActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, @this.Health);
    }
}
