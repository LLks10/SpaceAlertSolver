namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-01", "Pulse Ball", "Impulsbal")]
    public static Threat CreatePulseBall()
    {
        return new Threat()
        {
            MaxHealth = 5,
            Shield = 1,
            Speed = 2,
            ScoreWin = 4,
            ScoreLose = 2,
            _actX = ActDealDamageAll(1),
            _actY = ActDealDamageAll(1),
            _actZ = ActDealDamageAll(2),
        };
    }
}
