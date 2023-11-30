namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E2-02", "Scout", "Verkenner")]
    public static Threat CreateScout()
    {
        return new Threat()
        {
            MaxHealth = 3,
            Shield = 1,
            Speed = 2,
            ScoreWin = 6,
            ScoreLose = 3,
            _getDistance = HeavyLaserImmuneGetDistance,
        };
    }

    public static void ScoutOnBeaten(ref Threat @this)
    {
        if (!@this.Alive)
            @this.Game.ExternalDamageBonus = 0;
    }

    public static void ScoutActX(ref Threat @this)
    {
        @this.Game.ExternalDamageBonus = 1;
    }

    public static void ScoutActY(ref Threat @this)
    {
        foreach (int i in @this.Game.Threats.ExternalThreatIndices)
        {
            ref Threat threat = ref @this.Game.Threats[i];
            if (threat._actY == @this._actY)
                continue;
            @this.Game.MoveThreat(i, 1);
        }
    }

    public static void ScoutActZ(ref Threat @this)
    {
        @this.Game.DealInternalDamage(@this.Zone, 3);
    }
}
