namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-09", "Amoebe")]
    public static Threat CreateAmoebe()
    {
        return new Threat(8, 0, 2, 4, 2, actX: AmoebeHeal, actY: AmoebeHeal, actZ: ActDealDamage(5), getDistance: RocketImmuneGetDistance) { MaxHealth = 8 };
    }

    private static void AmoebeHeal(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 2, @this.MaxHealth);
    }
}
