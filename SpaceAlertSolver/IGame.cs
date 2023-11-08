namespace SpaceAlertSolver;

public interface IGame
{
    public void DealExternalDamage(int zone, int damage);

    public void DestroyShip();
}
