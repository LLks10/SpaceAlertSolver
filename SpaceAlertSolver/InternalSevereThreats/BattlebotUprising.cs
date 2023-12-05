using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    private const int FULL_BATTLEBOT_UPRISING_DAMAGE = (1 << Position.BOTTOM_LEFT_INDEX) | (1 << Position.TOP_RIGHT_INDEX);

    [InternalSevereThreat("SI1-06", "Battlebot Uprising", "Rebellerende Robots")]
    public static Threat CreateBattlebotUprising()
    {
        return new Threat()
        {
            MaxHealth = 4,
            Speed = 2,
            ScoreWin = 8,
            ScoreLose = 4,
        };
    }

    public static bool BattlebotUprisingIsTargetedBy(ref Threat @this, DamageSource damageSource, Position position)
    {
        return damageSource == DamageSource.RepairC && position == Position.BottomLeft || position == Position.TopRight;
    }

    public static void BattlebotUprisingOnSpawn(ref Threat @this)
    {
        @this.Game.AddMalfunctionC(Position.BottomLeft);
        @this.Game.AddMalfunctionC(Position.TopRight);
    }

    public static void BattlebotUprisingOnBeaten(ref Threat @this)
    {
        if (!@this.Alive)
        {
            @this.Game.RemoveMalfunctionC(Position.BottomLeft);
            @this.Game.RemoveMalfunctionC(Position.TopRight);
        }
    }

    public static void BattlebotUprisingDealDamage(ref Threat @this, DamageSource damageSource, int damage, int _, Position position)
    {
        Debug.Assert(damageSource == DamageSource.RepairC);
        Debug.Assert(position == Position.BottomLeft || position == Position.TopRight);
        @this.Health -= damage;
        @this.DamageBitmask |= 1 << position.PositionIndex;
        if (@this.DamageBitmask == FULL_BATTLEBOT_UPRISING_DAMAGE)
        {
            @this.Health -= 1;
            @this.DamageBitmask = -1; // block this from triggering again until turn end
        }

        @this.UpdateAlive();
    }

    public static void BattlebotUprisingEndTurn(ref Threat @this)
    {
        @this.DamageBitmask = 0;
    }

    public static void BattlebotUprisingActX(ref Threat @this)
    {
        for (int i = 0; i < @this.Game.Players.Length; i++)
        {
            if (@this.Game.Players[i].AndroidState == AndroidState.Alive)
            {
                Debug.Assert(@this.Game.Players[i].Alive);
                @this.Game.Players[i].Kill();
            }
        }
    }

    public static void BattlebotUprisingActY(ref Threat @this)
    {
        for (int i = 0; i < @this.Game.Players.Length; i++)
        {
            if (@this.Game.Players[i].Position == Position.BottomLeft || @this.Game.Players[i].Position == Position.TopRight)
            {
                @this.Game.Players[i].Kill();
            }
        }
    }

    public static void BattlebotUprisingActZ(ref Threat @this)
    {
        for (int i = 0; i < @this.Game.Players.Length; i++)
        {
            if (@this.Game.Players[i].Position != Position.TopMiddle)
            {
                @this.Game.Players[i].Kill();
            }
        }
    }
}
