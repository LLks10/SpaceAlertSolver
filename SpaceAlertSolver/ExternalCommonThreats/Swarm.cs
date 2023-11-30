using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    [ExternalCommonThreat("E2-04", "Swarm", "Zwerm")]
    public static Threat CreateSwarm()
    {
        return new Threat()
        {
            MaxHealth = 3,
            Shield = 0,
            Speed = 2,
            ScoreWin = 6,
            ScoreLose = 3,
            _actX = ActDealDamage(1),
            _getDistance = RocketImmuneGetDistance,
        };
    }

    public static void SwarmActY(ref Threat @this)
    {
        for (int i = 0; i < 3; i++)
        {
            int damage;
            if (@this.Zone == i)
                damage = 2;
            else
                damage = 1;
            @this.Game.DealExternalDamage(i, damage);
        }
    }

    public static void SwarmActZ(ref Threat @this)
    {
        for (int i = 0; i < 3; i++)
        {
            int damage;
            if (@this.Zone == i)
                damage = 3;
            else
                damage = 2;
            @this.Game.DealExternalDamage(i, damage);
        }
    }

    public static void SwarmProcessDamage(ref Threat @this)
    {
        Debug.Assert(@this.Damage > 0, "Should only process damage when there is some");
        int resultingDamage = @this.Damage - @this.Shield;
        @this.Damage = 0;
        if (resultingDamage <= 0)
            return;

        @this.Health--;
        @this.UpdateAlive();
    }
}
