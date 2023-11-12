namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-01", "Pulse Ball", "Impulsbal")]
    public static Threat CreatePulseBall()
    {
        return new Threat(5, 1, 2, 4, 2, actX: ActDealDamageAll(1), actY: ActDealDamageAll(1), actZ: ActDealDamageAll(2));
    }
}
