using System.Diagnostics;

namespace SpaceAlertSolver;

//count: 5

class CommandosRed : InThreat
{
    public CommandosRed(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 3;
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

    public override void ActX()
    {
        if (position < 3)
            position += 3;
        else
            position -= 3;

    }
    public override void ActY()
    {
        if(health < 2)
        {
            if (position != 2 && position != 5)
                position++;
        }
        else
        {
            int z = position % 3;
            game.ship.DealDamageIntern(z, 2);
        }            
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 4);
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position == position)
                game.ship.Players[i].Kill();
        }
    }
}

class CommandosBlue : InThreat
{
    public CommandosBlue(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 2;
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

    public override void ActX()
    {
        if (position < 3)
            position += 3;
        else
            position -= 3;

    }
    public override void ActY()
    {
        if (health < 2)
        {
            if (position != 0 && position != 3)
                position--;
        }
        else
        {
            int z = position % 3;
            game.ship.DealDamageIntern(z, 2);
        }
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 4);
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position == position)
                game.ship.Players[i].Kill();
        }
    }
}

class Alien : InThreat
{
    public Alien(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 4;
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
        if (position < 3)
            position += 3;
        else
            position -= 3;
        int c = 0;
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position == position)
                c++;
        }
        game.ship.DealDamageIntern(position % 3, c);
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
    public Eliminator(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 2;
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
        if (position != 0 && position != 3)
            position--;
        KillAll();
    }
    public override void ActY()
    {
        if (position < 3)
            position += 3;
        else
            position -= 3;
        KillAll();
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(position % 3, 3);
    }

    private void KillAll()
    {
        int z = position % 3;
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position % 3 == z && game.ship.Players[i].Position < 6 && game.ship.Players[i].AndroidState != AndroidState.Alive)
                game.ship.Players[i].Kill();
        }
    }
}

class SearchRobot : InThreat
{
    public SearchRobot(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 1;
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
        game.ship.DealDamageIntern(position % 3, 5);
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position == position)
                game.ship.Players[i].Kill();
        }
    }

    internal override bool DealDamage(int position, InternalDamageType damageType)
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
        int[] nearbyStation;
        switch (position)
        {
            case 0:
                nearbyStation = new int[] { 1, 3 };
                break;
            case 1:
                nearbyStation = new int[] { 0, 4, 2 };
                break;
            case 2:
                nearbyStation = new int[] { 1, 5 };
                break;
            case 3:
                nearbyStation = new int[] { 0, 4 };
                break;
            case 4:
                nearbyStation = new int[] { 3, 1, 5 };
                break;
            default: //case 5
                nearbyStation = new int[] { 2, 4 };
                break;
        }
        int best = -1;
        int score = int.MinValue;
        Player[] ps = game.ship.Players;
        //Count players and choose best station
        for(int i = 0; i < nearbyStation.Length; i++)
        {
            int c = 0;
            //Count players
            for(int j = 0; j < ps.Length; j++)
            {
                if (ps[j].Position == nearbyStation[i])
                    c++;
            }
            if (c > score)
            {
                best = i;
                score = c;
            }
            else if (c == score)
                best = -1;
        }

        if (best != -1)
            position = nearbyStation[best];
    }
}

class AtomicBomb : InThreat
{
    int damage = 0;
    public AtomicBomb(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 1;
        position = 4;
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
    internal override bool DealDamage(int position, InternalDamageType damageType)
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
    public RebelliousRobots(Game game, Trajectory traj, int time) : base(game, traj, time)
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
            if (game.ship.Players[i].AndroidState == AndroidState.Alive && game.ship.Players[i].Position < 6)
                game.ship.Players[i].Kill();
        }
    }
    public override void ActY()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position == 2 || game.ship.Players[i].Position == 3)
                game.ship.Players[i].Kill();
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position != 1 && game.ship.Players[i].Position < 6)
                game.ship.Players[i].Kill();
        }
    }
    internal override bool DealDamage(int position, InternalDamageType damageType)
	{
		if (damageType != vulnerability)
			return false;
        if (position != 2 && position != 3)
            return false;

		health--;
		if (!tookExtraDamage)
		{
			//Bonus damage if both stations activated
			hits[position - 2] = true;
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
    public SwitchedCables(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 4;
        position = 1;
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
    public OverstrainedEnergyNet(Game game, Trajectory traj, int time) : base(game, traj, time)
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
    internal override bool DealDamage(int position, InternalDamageType damageType)
    {
        if (damageType == vulnerability)
        {
            //Check if at any of the two positions
            if (position == 3 || position == 4 || position == 5)
            {
                health--;
                //Bonus damage if both stations activated
                hits[position - 3] = true;
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
    public Fissure(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 6;
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
    public Infection(Game game, Trajectory traj, int time) : base(game, traj, time)
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
            if (isActive[game.ship.Players[i].Position])
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
            if (isActive[game.ship.Players[i].Position])
                game.ship.Players[i].Kill();
        }
        for(int i = 0; i < 6; i++)
        {
            if (isActive[i])
                game.ship.StationStatus[i] |= 2;
        }
    }
    internal override bool DealDamage(int position, InternalDamageType damageType)
    {
        if (damageType == vulnerability && isActive[position])
        {
            isActive[position] = false;
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
