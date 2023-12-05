using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    [InternalSevereThreat("SI1-03", "Alien")]
    public static Threat CreateAlien()
    {
        return new Threat()
        {
            MaxHealth = 2,
            Speed = 2,
            Position = Position.BottomMiddle,
            ScoreWin = 8,
            ScoreLose = 0,
            _isTargetedBy = IsTargetedByRobots,
            _dealInternalDamage = DefaultInternalDealDamage,
        };
    }

    public static void AlienActX(ref Threat @this)
    {
        @this._dealInternalDamage = InternalDealDamageFightback;
    }

    public static void AlienActY(ref Threat @this)
    {
        Debug.Assert(@this.Position.GetElevator() != @this.Position);
        @this.Position = @this.Position.GetElevator();
        @this.DealDamageForEachPlayerInStation();
    }

    public static void AlienActZ(ref Threat @this)
    {
        @this.Game.DestroyShip();
    }

    private void DealDamageForEachPlayerInStation()
    {
        int damage = 0;
        for (int i = 0; i < Game.Players.Length; i++)
        {
            if (Game.Players[i].Alive && Game.Players[i].Position == Position)
                damage++;
        }
        if (damage > 0)
            Game.DealInternalDamage(Position.Zone, damage);
    }
}
