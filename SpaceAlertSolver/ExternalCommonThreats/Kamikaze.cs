namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E2-01", "Kamikaze")]
    public static Threat CreateKamikaze()
    {
        return new Threat()
        {
            MaxHealth = 5,
            Shield = 2,
            Speed = 4,
            ScoreWin = 6,
            ScoreLose = 3,
            _actX = KamikazeSpeedup,
            _actY = KamikazeSpeedup,
            _actZ = ActDealDamage(6),
        };
    }

    private static void KamikazeSpeedup(ref Threat @this)
    {
        @this.Speed++;
        @this.Shield--;
    }
}
