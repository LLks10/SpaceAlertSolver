namespace SpaceAlertSolver;

public partial struct Threat
{
    private bool HasShield { readonly get => _value1 > 0; set => _value1 = value ? 1 : 0; }

    [ExternalCommonThreat("E1-06", "Cryoshield Fighter", "Cryoschild Jager")]
    public static Threat CreateCryoshieldFighter()
    {
        return new Threat()
        {
            MaxHealth = 4,
            Shield = 1,
            Speed = 3,
            ScoreWin = 4,
            ScoreLose = 2,
            _actX = ActDealDamage(1),
            _actY = ActDealDamage(2),
            _actZ = ActDealDamage(2),
            _processDamageOrEndTurn = CryoshieldProcessDamage,
            HasShield = true,
        };
    }

    private static void CryoshieldProcessDamage(ref Threat @this)
    {
        if (@this.HasShield && @this.Damage > 0)
        {
            @this.HasShield = false;
            @this.Damage = 0;
            return;
        }
        DefaultExternalProcessDamage(ref @this);
    }
}
