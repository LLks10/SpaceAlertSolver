namespace SpaceAlertSolver;

public interface IGame
{
    public Player[] Players { get; }

    public void DealExternalDamage(int zone, int damage);

    public void DealInternalDamage(int zone, int damage);

    public void DestroyShip();
}
