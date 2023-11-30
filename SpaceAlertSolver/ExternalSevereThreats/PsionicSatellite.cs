namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalSevereThreat("SE2-03", "Psionic Satellite", "Psionische Satelliet")]
    public static Threat CreatePsionicSatellite()
    {
        return new Threat()
        {
            MaxHealth = 5,
            Shield = 2,
            Speed = 2,
            ScoreWin = 12,
            ScoreLose = 6,
        };
    }

    public static int PsionicSatelliteGetDistance(ref Threat @this, DamageSource _)
    {
        if (@this.Distance >= Trajectory.RANGE_3_START)
            return int.MaxValue;
        return @this.Distance;
    }

    public static void PsionicSatelliteActX(ref Threat @this)
    {
        for (int i = 0; i < @this.Game.Players.Length; i++)
        {
            ref Player player = ref @this.Game.Players[i];
            if (player.Position.Zone == @this.Zone)
                player.DelayNext();
        }
    }

    public static void PsionicSatelliteActY(ref Threat @this)
    {
        for (int i = 0; i < @this.Game.Players.Length; i++)
        {
            ref Player player = ref @this.Game.Players[i];
            if (player.Position.IsInShip())
                player.DelayNext();
        }
    }

    public static void PsionicSatelliteActZ(ref Threat @this)
    {
        for (int i = 0; i < @this.Game.Players.Length; i++)
        {
            ref Player player = ref @this.Game.Players[i];
            if (player.Position.IsInShip())
                player.Kill();
        }
    }
}
