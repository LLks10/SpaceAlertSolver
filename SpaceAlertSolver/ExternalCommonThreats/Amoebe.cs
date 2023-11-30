namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-09", "Amoebe")]
    public static Threat CreateAmoebe()
    {
        return new Threat()
        {
            MaxHealth = 8,
            Shield = 0,
            Speed = 2,
            ScoreWin = 4,
            ScoreLose = 2,
            _actX = AmoebeHeal,
            _actY = AmoebeHeal,
            _actZ = ActDealDamage(5),
            _getDistance = RocketImmuneGetDistance,
        };
    }

    private static void AmoebeHeal(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 2, @this.MaxHealth);
    }
}
