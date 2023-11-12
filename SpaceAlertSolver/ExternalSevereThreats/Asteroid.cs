namespace SpaceAlertSolver;

public partial struct Threat
{
    public int RevengeDamage
    {
        get => _value1;
        set => _value1 = value;
    }

    [ExternalSevereThreat("SE1-08", "Asteroid", "Asteroïde")]
    public static Threat CreateAsteroid()
    {
        SimpleDelegate revengeDelegate = IncreaseRevengeAct(2);
        return new Threat(9, 0, 3, 8, 4, actX: revengeDelegate, actY: revengeDelegate, actZ: AsteroidLikeActZ, getDistance: RocketImmuneGetDistance, onBeaten: DealAsteroidRevengeDamage) { RevengeDamage = 0 };
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
