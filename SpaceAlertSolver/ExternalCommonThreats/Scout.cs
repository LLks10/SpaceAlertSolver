namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E2-02", "Scout", "Verkenner")]
    public static Threat CreateScout()
    {
        return new(3, 1, 2, 6, 3, getDistance: HeavyLaserImmuneGetDistance);
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
        for (int i = @this.Game.Threats.Count - 1; i >= 0; i--)
        {
            ref Threat threat = ref @this.Game.Threats[i];
            if (!threat.IsExternal || threat._actY == @this._actY)
                continue;
            @this.Game.MoveThreat(i, 1);
        }
    }

    public static void ScoutActZ(ref Threat @this)
    {
        @this.Game.DealInternalDamage(@this.Zone, 3);
    }
}
