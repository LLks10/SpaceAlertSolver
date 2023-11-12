namespace SpaceAlertSolver;

public interface IGame
{
    public Player[] Players { get; }

    public RefList<Threat> Threats { get; }

    public void MoveThreat(int threatId, int speed);

    public void DealExternalDamage(int zone, int damage);

    public void DealInternalDamage(int zone, int damage);

    public void DestroyShip();

    public int ExternalDamageBonus { set; }
}
