using System.Diagnostics;

namespace SpaceAlertSolver;

//count: 5

class CommandosRed : InThreat
{
    public CommandosRed(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 2;
        position = Position.BottomLeft;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public CommandosRed() { }

    internal override InThreat Clone(Game game)
    {
        CommandosRed clone = new CommandosRed();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX() => TakeElevator();
    public override void ActY()
    {
        if(health < 2)
            MoveRight();
        else
            game.ship.DealDamageIntern(position.Zone, 2);         
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(position.Zone, 4);
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (AtPosition(game.ship.Players[i].Position))
                game.ship.Players[i].Kill();
        }
    }
}

class CommandosBlue : InThreat
{
    public CommandosBlue(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 2;
        position = Position.TopRight;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public CommandosBlue() { }

    internal override InThreat Clone(Game game)
    {
        CommandosBlue clone = new CommandosBlue();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX() => TakeElevator();
    public override void ActY()
    {
        if (health < 2)
            MoveLeft();
        else
            game.ship.DealDamageIntern(position.Zone, 2);
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(position.Zone, 4);
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (AtPosition(game.ship.Players[i].Position))
                game.ship.Players[i].Kill();
        }
    }
}

class Alien : InThreat
{
    public Alien(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 2;
        position = Position.BottomMiddle;
        speed = 2;
        scoreLose = 0;
        scoreWin = 8;
        vulnerability = InternalDamageType.Android;
    }

    public Alien() { }

    internal override InThreat Clone(Game game)
    {
        Alien clone = new Alien();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        fightBack = true;
    }
    public override void ActY()
    {
        TakeElevator();
        int c = 0;
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (AtPosition(game.ship.Players[i].Position))
                c++;
        }
        game.ship.DealDamageIntern(position.Zone, c);
    }
    public override void ActZ()
    {
        game.ship.Damage[0] = 7;
        game.ship.Damage[1] = 7;
        game.ship.Damage[2] = 7;
    }
}

class Eliminator : InThreat
{
    public Eliminator(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 2;
        position = Position.TopRight;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public Eliminator() { }

    internal override InThreat Clone(Game game)
    {
        Eliminator clone = new Eliminator();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        Position left = position.GetLeft();
        if (position != left)
        {
            position = left;
            KillAll();
        }
    }
    public override void ActY()
    {
        Debug.Assert(position != Position.Space);
        TakeElevator();
        KillAll();
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(position.Zone, 3);
    }

    private void KillAll()
    {
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (AtPosition(game.ship.Players[i].Position) && game.ship.Players[i].Position.IsInShip() && game.ship.Players[i].AndroidState != AndroidState.Alive)
                game.ship.Players[i].Kill();
        }
    }
}

class SearchRobot : InThreat
{
    public SearchRobot(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 2;
        position = Position.TopMiddle;
        speed = 2;
        scoreLose = 6;
        scoreWin = 15;
        vulnerability = InternalDamageType.Android;
    }

    public SearchRobot() { }

    internal override InThreat Clone(Game game)
    {
        SearchRobot clone = new SearchRobot();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        MoveToClosest();
    }
    public override void ActY()
    {
        MoveToClosest();
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(position.Zone, 5);
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (AtPosition(game.ship.Players[i].Position))
                game.ship.Players[i].Kill();
        }
    }

    internal override bool DealDamage(Position position, InternalDamageType damageType)
    {
        if (damageType == vulnerability && AtPosition(position))
        {
            health--;
            if (health <= 0)
            {
                game.ship.GetCurrentTurnPlayer().Kill();

                alive = false;
                beaten = true;
            }
            return true;
        }
        return false;
    }

    private void MoveToClosest()
    {
        Position best = position;
        int score = int.MinValue;

        void TestPosition(in Position pos)
        {
            int c = 0;
            //Count players
            for (int i = 0; i < game.ship.Players.Length; i++)
            {
                if (game.ship.Players[i].Position == pos)
                    c++;
            }
            if (c > score)
            {
                best = pos;
                score = c;
            }
            else if (c == score)
                best = position;
        }

        Position left = position.GetLeft();
        if (position != left)
            TestPosition(in left);

        Position right = position.GetRight();
        if (position != right)
            TestPosition(in right);

        Position elevated = position.GetElevator();
        Debug.Assert(position != elevated, "Elevator must always have an effect for this threat");
        TestPosition(in elevated);

        position = best;
    }
}

class AtomicBomb : InThreat
{
    int damage = 0;
    public AtomicBomb(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 1;
        position = Position.BottomMiddle;
        game.ship.CDefect[4]++;
        speed = 4;
        scoreLose = 0;
        scoreWin = 12;
        vulnerability = InternalDamageType.C;
    }

    public AtomicBomb() { }

    internal override InThreat Clone(Game game)
    {
        AtomicBomb clone = new AtomicBomb();
        clone.CloneThreat(this, game);
        clone.damage = damage;
        return clone;
    }
    public override void ActX()
    {
        speed++;
    }
    public override void ActY()
    {
        speed++;
    }
    public override void ActZ()
    {
        game.ship.Damage[0] = 7;
        game.ship.Damage[1] = 7;
        game.ship.Damage[2] = 7;
    }
    internal override bool DealDamage(Position position, InternalDamageType damageType)
    {
        if (damageType == vulnerability && AtPosition(position))
        {
            damage++;
            if(damage >= 3)
            {
                health--;
                beaten = true;
                alive = false;
                game.ship.CDefect[4]--;
            }
            return true;
        }
        return false;
    }
    public override bool ProcessTurnEnd()
    {
        if (beaten)
            return true;
        damage = 0;
        return false;
    }
}

class RebelliousRobots : InThreat
{
    bool tookExtraDamage;
    bool[] hits = new bool[2];
    public RebelliousRobots(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 4;
        game.ship.CDefect[2]++;
        game.ship.CDefect[3]++;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InternalDamageType.C;
    }

    public RebelliousRobots() { }

    internal override InThreat Clone(Game game)
    {
        RebelliousRobots clone = new RebelliousRobots();
        clone.CloneThreat(this, game);
        clone.tookExtraDamage = tookExtraDamage;
        clone.hits = hits.ToArray();
        return clone;
    }
    public override void OnClear()
    {
        game.ship.CDefect[2]--;
        game.ship.CDefect[3]--;
    }
    public override void ActX()
    {
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].AndroidState == AndroidState.Alive && game.ship.Players[i].Position.IsInShip())
                game.ship.Players[i].Kill();
        }
    }
    public override void ActY()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position == Position.TopRight || game.ship.Players[i].Position == Position.BottomLeft)
                game.ship.Players[i].Kill();
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position != Position.TopMiddle && game.ship.Players[i].Position.IsInShip())
                game.ship.Players[i].Kill();
        }
    }
    internal override bool DealDamage(Position position, InternalDamageType damageType)
	{
		if (damageType != vulnerability)
			return false;
        if (position != Position.TopRight && position != Position.BottomLeft)
            return false;

		health--;
		if (!tookExtraDamage)
		{
			//Bonus damage if both stations activated
			hits[position.PositionIndex - 2] = true;
			if (hits[0] && hits[1])
			{
				health--;
				tookExtraDamage = true;
			}
		}
		//Check death
		if (health <= 0)
		{
			beaten = true;
			alive = false;
			OnClear();
		}
		return true;
	}
	public override bool ProcessTurnEnd()
    {
        if (beaten)
            return true;
        tookExtraDamage = false;
        hits[0] = false;
        hits[1] = false;
        return false;
    }
}

class SwitchedCables : InThreat
{
    public SwitchedCables(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 4;
        position = Position.TopMiddle;
        game.ship.BDefect[1]++;
        speed = 3;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InternalDamageType.B;
    }

    public SwitchedCables() { }

    internal override InThreat Clone(Game game)
    {
        SwitchedCables clone = new SwitchedCables();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void OnClear()
    {
        game.ship.BDefect[1]--;
    }

    public override void ActX()
    {
        game.BranchReactorFull(1);
        game.BranchConditional(1, Defects.shield);
        game.ship.Shields[1] += game.ship.Reactors[1];
        game.ship.Reactors[1] = 0;
        int excess = game.ship.Shields[1] - game.ship.ShieldsCap[1];
        if(excess > 0)
        {
            game.ship.Shields[1] = game.ship.ShieldsCap[1];
            game.ship.DealDamageIntern(1, excess);
        }

    }
    public override void ActY()
    {
        game.BranchShieldFull(1);
        game.ship.DealDamageIntern(1, game.ship.Shields[1]);
        game.ship.Shields[1] = 0;
    }
    public override void ActZ()
    {
        game.BranchReactorFull(0);
        game.BranchReactorFull(1);
        game.BranchReactorFull(2);

        for (int i = 0; i < 3; i++)
        {
            game.ship.DealDamageIntern(i, game.ship.Reactors[i]);
            game.ship.Reactors[i] = 0;
        }
    }
}

class OverstrainedEnergyNet : InThreat
{
    bool tookExtraDamage;
    bool[] hits = new bool[3];
    public OverstrainedEnergyNet(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 7;
        game.ship.BDefect[3]++;
        game.ship.BDefect[4]++;
        game.ship.BDefect[5]++;
        speed = 3;
        scoreLose = 6;
        scoreWin = 12;
        vulnerability = InternalDamageType.B;
    }

    public OverstrainedEnergyNet() { }

    internal override InThreat Clone(Game game)
    {
        OverstrainedEnergyNet clone = new OverstrainedEnergyNet();
        clone.CloneThreat(this, game);
        clone.tookExtraDamage = tookExtraDamage;
        clone.hits = hits.ToArray();
        return clone;
    }
    public override void OnClear()
    {
        game.ship.BDefect[3]--;
        game.ship.BDefect[4]--;
        game.ship.BDefect[5]--;
    }
    public override void ActX()
    {
        game.BranchReactorFull(1);
        game.ship.Reactors[1] = Math.Max(0, game.ship.Reactors[1] - 2);
    }
    public override void ActY()
    {
        game.BranchReactorFull(0);
        game.BranchReactorFull(1);
        game.BranchReactorFull(2);
        for (int i = 0; i < 3; i++)
            game.ship.Reactors[i] = Math.Max(0, game.ship.Reactors[i] - 1);
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(0, 3);
        game.ship.DealDamageIntern(1, 3);
        game.ship.DealDamageIntern(2, 3);
    }
    internal override bool DealDamage(Position position, InternalDamageType damageType)
    {
        if (damageType == vulnerability)
        {
            //Check if at any of the two positions
            if (position.IsBottom())
            {
                health--;
                //Bonus damage if both stations activated
                hits[position.PositionIndex - 3] = true;
                if (!tookExtraDamage)
                {
                    if (hits[0] && hits[1] && hits[2])
                    {
                        health -= 2;
                        tookExtraDamage = true;
                    }
                }
                //Check death
                if (health <= 0)
                {
                    beaten = true;
                    alive = false;
                    OnClear();
                }
                return true;
            }
        }
        return false;
    }
    public override bool ProcessTurnEnd()
    {
        if (beaten)
            return true;
        tookExtraDamage = false;
        hits[0] = false;
        hits[1] = false;
        hits[2] = false;
        return false;
    }
}

class Fissure : InThreat
{
    public Fissure(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 2;
        position = Position.Space;
        game.ship.CDefect[6]++;
        speed = 2;
        scoreLose = 0;
        scoreWin = 8;
        vulnerability = InternalDamageType.C;
    }

    public Fissure() { }

    internal override InThreat Clone(Game game)
    {
        Fissure clone = new Fissure();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void OnClear()
    {
        game.ship.CDefect[6]--;
        game.ship.Fissured[0] = false;
        game.ship.Fissured[1] = false;
        game.ship.Fissured[2] = false;
    }
    public override void ActX()
    {
        game.ship.Fissured[0] = true;
    }
    public override void ActY()
    {
        game.ship.Fissured[0] = true;
        game.ship.Fissured[1] = true;
        game.ship.Fissured[2] = true;
    }
    public override void ActZ()
    {
        game.ship.Damage[0] = 7;
        game.ship.Damage[1] = 7;
        game.ship.Damage[2] = 7;
    }
}

class Infection : InThreat
{
    bool[] isActive = new bool[] {true,false,true,true,false,true,false };
    public Infection(IGame game, Trajectory traj) : base(game, traj)
    {
        health = 3;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
        vulnerability = InternalDamageType.Android;
    }

    public Infection() { }

    internal override InThreat Clone(Game game)
    {
        Infection clone = new Infection();
        clone.CloneThreat(this, game);
        clone.isActive = isActive.ToArray();
        return clone;
    }

    public override void ActX()
    {
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (isActive[game.ship.Players[i].Position.PositionIndex])
                game.ship.Players[i].DelayNext();
        }
    }
    public override void ActY()
    {
        for (int i = 0; i < 6; i++)
        {
            if (isActive[i])
                game.ship.DealDamageIntern(i % 3, 1);
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (isActive[game.ship.Players[i].Position.PositionIndex])
                game.ship.Players[i].Kill();
        }
        for(int i = 0; i < 6; i++)
        {
            if (isActive[i])
                game.ship.StationStatus[i] |= 2;
        }
    }
    internal override bool DealDamage(Position position, InternalDamageType damageType)
    {
        if (damageType == vulnerability && isActive[position.PositionIndex])
        {
            isActive[position.PositionIndex] = false;
            health--;
            //Check death
            if (health <= 0)
            {
                beaten = true;
                alive = false;
                OnClear();
            }
            return true;
        }
        return false;
    }
}
