using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    [InternalSevereThreat("SI1-01", "Commandos (red)", "Commando's (rood)")]
    public static Threat CreateCommandosRed()
    {
        return new Threat()
        {
            MaxHealth = 2,
            Speed = 2,
            Position = Position.BottomLeft,
            ScoreWin = 8,
            ScoreLose = 4,
            _isTargetedBy = IsTargetedByRobots,
            _dealInternalDamage = InternalDealDamageFightback,
            _actX = CommandosActX,
            _actZ = CommandosActZ,
        };
    }

    [InternalSevereThreat("SI1-02", "Commandos (blue)", "Commando's (blauw)")]
    public static Threat CreateCommandosBlue()
    {
        return new Threat()
        {
            MaxHealth = 2,
            Speed = 2,
            Position = Position.TopRight,
            ScoreWin = 8,
            ScoreLose = 4,
            _isTargetedBy = IsTargetedByRobots,
            _dealInternalDamage = InternalDealDamageFightback,
            _actX = CommandosActX,
            _actZ = CommandosActZ,
        };
    }

    private static void CommandosActX(ref Threat @this)
    {
        @this.Position = @this.Position.GetElevator();
    }

    public static void CommandosRedActY(ref Threat @this)
    {
        if (@this.Health < @this.MaxHealth)
        {
            @this.Position = @this.Position.GetRight();
        }
        else
        {
            @this.Game.DealInternalDamage(@this.Position.Zone, 2);
        }
    }

    public static void CommandosBlueActY(ref Threat @this)
    {
        if (@this.Health < @this.MaxHealth)
        {
            @this.Position = @this.Position.GetLeft();
        }
        else
        {
            @this.Game.DealInternalDamage(@this.Position.Zone, 2);
        }
    }

    private static void CommandosActZ(ref Threat @this)
    {
        @this.Game.DealInternalDamage(@this.Position.Zone, 4);
        @this.KillPlayersInStation(@this.Position);
    }

    private void KillPlayersInStation(Position station)
    {
        for (int i = 0; i < Game.Players.Length; i++)
        {
            if (Game.Players[i].Position == station)
            {
                Game.Players[i].Kill();
            }
        }
    }
}
