namespace SpaceAlertSolver;

//External threat parent class
public abstract class InThreat
{
    public int health, speed, distance, scoreWin, scoreLose;
    public Position position;
    public Trajectory trajectory;
    private protected InternalDamageType vulnerability;
    public bool alive, beaten, fightBack;
    public IGame game;

    bool started_move = false;
    int distance_moved;
    int current_speed;

    internal InThreat(IGame game, Trajectory traj)
    {
        this.game = game;
        this.trajectory = traj;
        alive = true;

        //Get trajectory information
        distance = traj.maxDistance;
    }
    public InThreat() { }

    internal abstract InThreat Clone(Game game);

    private protected virtual void CloneThreat(InThreat other, Game game)
    {
        health = other.health;
        speed = other.speed;
        distance = other.distance;
        position = other.position;
        scoreWin = other.scoreWin;
        scoreLose = other.scoreLose;
        time = other.time;
        trajectory = other.trajectory;
        vulnerability = other.vulnerability;
        alive = other.alive;
        beaten = other.beaten;
        fightBack = other.fightBack;

        started_move = other.started_move;
        distance_moved = other.distance_moved;
        current_speed = other.current_speed;

        this.game = game;
    }

    internal virtual bool DealDamage(Position position, InternalDamageType damageType)
    {
        if(damageType == vulnerability && AtPosition(position))
        {
            health--;
            if(health <= 0)
            {
                alive = false;
                beaten = true;
                OnClear();
            }
            return true;
        }
        return false;
    }

    public virtual void OnClear() { }

    public virtual bool AtPosition(Position position)
    {
        return position == this.position;
    }

    public virtual bool ProcessTurnEnd()
    {
        return false;
    }

    public virtual void Move()
    {
        Move(speed);
    }

    public virtual void Move(int mSpd)
    {
        if (!started_move)
        {
            current_speed = mSpd;
            distance_moved = 0;
            started_move = true;
        }
        while (distance_moved < current_speed && distance - distance_moved > 0)
        {
            switch (trajectory.actions[distance - distance_moved - 1])
            {
                case 1:
                    ActX();
                    break;
                case 2:
                    ActY();
                    break;
                case 3:
                    ActZ();
                    break;
            }
            distance_moved++;
        }
        started_move = false;

        //Set new position
        distance -= mSpd;
        if (distance <= 0)
            beaten = true;
    }

    protected void MoveLeft() => position = position.GetLeft();

    protected void MoveRight() => position = position.GetRight();

    protected void TakeElevator() => position = position.GetElevator();

    public virtual void ActX() { }
    public virtual void ActY() { }
    public virtual void ActZ() { }
}

internal enum InternalDamageType
{
    B,
    C,
    Android,
}
