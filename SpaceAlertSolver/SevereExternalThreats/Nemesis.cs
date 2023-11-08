namespace SpaceAlertSolver;

internal partial struct Threat
{
    [ExternalSevereThreat("SE2-05", "Nemesis")]
    public static Threat CreateNemesis()
    {
        return new Threat(9, 1, 3, 12, 0);
    }

    public static void NemesisActX(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 1);
        @this.Health--;
        @this.UpdateAlive();
    }

    public static void NemesisActY(ref Threat @this)
    {
        @this.Game.DealExternalDamage(@this.Zone, 2);
        @this.Health -= 2;
        @this.UpdateAlive();
    }

    public static void NemesisActZ(ref Threat @this)
    {
        @this.Game.DestroyShip();
    }

    public static void NemesisProcessDamage(ref Threat @this)
    {
        int health = @this.Health;
        DefaultExternalProcessDamage(ref @this);
        if (@this.Health < health)
        {
            @this.Game.DealExternalDamage(0, 1);
            @this.Game.DealExternalDamage(1, 1);
            @this.Game.DealExternalDamage(2, 1);
        }
    }
}
