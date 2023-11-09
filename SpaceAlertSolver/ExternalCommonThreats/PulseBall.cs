namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ExternalCommonThreat("E1-01", "Pulse Ball", "Impulsbal")]
    public static Threat CreatePulseBall()
    {
        return new Threat(5, 1, 2, 4, 2, actX: PulseBallActXY, actY: PulseBallActXY);
    }

    private static void PulseBallActXY(ref Threat @this)
    {
        @this.Game.DealExternalDamage(0, 1);
        @this.Game.DealExternalDamage(1, 1);
        @this.Game.DealExternalDamage(2, 1);
    }

    public static void PulseBallActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(0, 2);
        @this.Game.DealExternalDamage(1, 2);
        @this.Game.DealExternalDamage(2, 2);
    }
}
