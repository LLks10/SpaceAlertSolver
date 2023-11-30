namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-10", "Meteoroid", "Meteoroïde")]
    public static Threat CreateMeteoroid()
    {
        return new Threat()
        {
            MaxHealth = 5,
            Shield = 0,
            Speed = 5,
            ScoreWin = 4,
            ScoreLose = 2,
            _actX = Blank,
            _actY = Blank,
            _actZ = AsteroidLikeActZ,
            _getDistance = RocketImmuneGetDistance,
        };
    }
}
