namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-05", "Gunship", "Slagschip")]
    public static Threat CreateGunship()
    {
        return new Threat()
        {
            MaxHealth = 5,
            Shield = 2,
            Speed = 2,
            ScoreWin = 4,
            ScoreLose = 2,
            _actX = ActDealDamage(2),
            _actY = ActDealDamage(2),
            _actZ = ActDealDamage(3),
        };
    }
}
