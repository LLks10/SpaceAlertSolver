namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E2-01", "Kamikaze")]
    public static Threat CreateKamikaze()
    {
        return new(5, 2, 4, 6, 3, KamikazeSpeedup, KamikazeSpeedup, ActDealDamage(6));
    }

    private static void KamikazeSpeedup(ref Threat @this)
    {
        @this.Speed++;
        @this.Shield--;
    }
}
