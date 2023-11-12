using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    [InternalSevereThreat("SI2-01", "Executioner", "Eliminator")]
    public static Threat CreateExecutioner()
    {
        return new(2, 2, Position.TopRight, 12, 6);
    }

    public static void ExecutionerActX(ref Threat @this)
    {
        Debug.Assert(@this.Position.GetLeft() != @this.Position);
        @this.Position = @this.Position.GetLeft();
        @this.KillAllPlayersInStationWithoutRobots();
    }

    public static void ExecutionerActY(ref Threat @this)
    {
        Debug.Assert(@this.Position.GetElevator() != @this.Position);
        @this.Position = @this.Position.GetElevator();
        @this.KillAllPlayersInStationWithoutRobots();
    }

    public static void ExecutionerActZ(ref Threat @this)
    {
        @this.Game.DealInternalDamage(@this.Position.Zone, 3);
    }

    private void KillAllPlayersInStationWithoutRobots()
    {
        for (int i = 0; i < Game.Players.Length; i++)
        {
            if (Game.Players[i].Position == Position && Game.Players[i].AndroidState != AndroidState.Alive)
                Game.Players[i].Kill();
        }
    }
}
