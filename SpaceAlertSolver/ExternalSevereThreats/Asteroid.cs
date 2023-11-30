namespace SpaceAlertSolver;

public partial struct Threat
{
    private int RevengeDamage
    {
        get => _value1;
        set => _value1 = value;
    }

    [ExternalSevereThreat("SE1-08", "Asteroid", "Asteroïde")]
    public static Threat CreateAsteroid()
    {
        SimpleDelegate revengeDelegate = IncreaseRevengeAct(2);
        return new Threat()
        {
            MaxHealth = 9,
            Shield = 0,
            Speed = 3,
            ScoreWin = 8,
            ScoreLose = 4,
            _actX = revengeDelegate,
            _actY = revengeDelegate,
            _actZ = AsteroidLikeActZ,
            _getDistance = RocketImmuneGetDistance,
            _onBeaten = DealAsteroidRevengeDamage,
            RevengeDamage = 0,
        };
    }

    private static SimpleDelegate IncreaseRevengeAct(int revengeDamage)
    {
        return (ref Threat @this) => { @this.RevengeDamage += revengeDamage; };
    }

    private static void AsteroidLikeActZ(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, @this.Health);
    }

    private static void DealAsteroidRevengeDamage(ref Threat @this)
    {
        if (!@this.Alive)
            @this.Game.DealExternalDamage(@this.Zone, @this.RevengeDamage);
    }
}
