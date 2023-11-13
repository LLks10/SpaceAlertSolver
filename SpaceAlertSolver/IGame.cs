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

    public void SpillEnergy(Position position, int amount);

    public void AddMalfunctionA(Position position);

    public void AddMalfunctionB(Position position);

    public void AddMalfunctionC(Position position);

    public void RemoveMalfunctionA(Position position);

    public void RemoveMalfunctionB(Position position);

    public void RemoveMalfunctionC(Position position);
}
