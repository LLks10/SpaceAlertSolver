namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-10", "Meteoroid", "Meteoroïde")]
    public static Threat CreateMeteoroid()
    {
        return new Threat(5, 0, 5, 4, 2, actX: Blank, actY: Blank, actZ: AsteroidLikeActZ, getDistance: RocketImmuneGetDistance);
    }
}
