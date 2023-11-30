namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E1-08", "Armored Grappler", "Gepantserde Grijper")]
    public static Threat CreateArmoredGrappler()
    {
        return new Threat()
        {
            MaxHealth = 4,
            Shield = 3,
            Speed = 2,
            ScoreWin = 4,
            ScoreLose = 2,
            _actX = ActDealDamage(1),
            _actZ = ActDealDamage(4),
        };
    }

    public static void ArmoredGrapplerActY(ref Threat @this)
    {
        @this.Health = Math.Min(@this.Health + 1, @this.MaxHealth);
    }
}
