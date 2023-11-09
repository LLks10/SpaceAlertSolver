namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ExternalCommonThreat("E1-05", "Gunship", "Slagschip")]
    public static Threat CreateGunship()
    {
        return new Threat(5, 2, 2, 4, 2, actX: ActDealDamage(2), actY: ActDealDamage(2), actZ: ActDealDamage(3));
    }
}
