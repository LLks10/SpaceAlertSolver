namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalSevereThreat("SE1-05", "Cryoshield Frigate", "Cryoschild Fregat")]
    public static Threat CreateCryoshieldFrigate()
    {
        return new Threat()
        {
            MaxHealth = 7,
            Shield = 1,
            Speed = 2,
            ScoreWin = 8,
            ScoreLose = 4,
            _actX = ActDealDamage(2),
            _actY = ActDealDamage(3),
            _actZ = ActDealDamage(4),
            _processDamageOrEndTurn = CryoshieldProcessDamage,
            HasShield = true,
        };
    }
}
