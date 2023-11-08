namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ExternalCommonThreat("E1-05", "Gunship", "Slagschip")]
    public static Threat CreateGunship()
    {
        return new Threat(5, 2, 2, 4, 2, actX: GunshipActXY, actY: GunshipActXY);
    }

    private static void GunshipActXY(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 2);
    }

    public static void GunshipActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 3);
    }
}
