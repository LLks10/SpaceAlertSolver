namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalSevereThreat("SE1-05", "Cryoshield Frigate", "Cryoschild Fregat")]
    public static Threat CreateCryoshieldFrigate()
    {
        return new(7, 1, 2, 8, 4, ActDealDamage(2), ActDealDamage(3), ActDealDamage(4), processDamage: CryoshieldProcessDamage);
    }
}
