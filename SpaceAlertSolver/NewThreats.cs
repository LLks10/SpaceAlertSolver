namespace SpaceAlertSolver;

internal class PhasingFighter : ExThreat
{
	bool phased = false;
	public PhasingFighter(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
	{
		health = 4;
		shield = 2;
		speed = 3;
		scoreLose = 3;
		scoreWin = 6;
	}

	public PhasingFighter() { }

	public override int GetDistance(int range, ExDmgSource source)
	{
		if (phased)
			return int.MaxValue;
		return base.GetDistance(range, source);
	}

	public override void Move(int mSpd)
	{
		base.Move(mSpd);
		phased = !phased;
	}

	public override void ActX()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, 1);
	}
	public override void ActY()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, phased ? 1 : 2);
	}
	public override void ActZ()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, phased ? 2 : 3);
	}

	internal override ExThreat Clone(Game game)
	{
		var clone = new PhasingFighter();
		clone.CloneThreat(this, game);
		clone.phased = phased;
		return clone;
	}
}

internal class PhasingFrigate : ExThreat
{
	bool phased = false;
	public PhasingFrigate(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
	{
		health = 7;
		shield = 2;
		speed = 2;
		scoreLose = 6;
		scoreWin = 12;
	}

	public PhasingFrigate() { }

	public override int GetDistance(int range, ExDmgSource source)
	{
		if (phased)
			return int.MaxValue;
		return base.GetDistance(range, source);
	}

	public override void Move(int mSpd)
	{
		base.Move(mSpd);
		phased = !phased;
	}

	public override void ActX()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, phased ? 1 : 2);
	}
	public override void ActY()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, phased ? 2 : 3);
	}
	public override void ActZ()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, phased ? 3 : 4);
	}

	internal override ExThreat Clone(Game game)
	{
		var clone = new PhasingFrigate();
		clone.CloneThreat(this, game);
		clone.phased = phased;
		return clone;
	}
}

internal class PlasmaticNeedleship : ExThreat
{
	public PlasmaticNeedleship (Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
	{
		health = 3;
		shield = 1;
		speed = 3;
		scoreLose = 4;
		scoreWin = 8;
	}

	public PlasmaticNeedleship() { }

	public override int GetDistance(int range, ExDmgSource source)
	{
		if (source == ExDmgSource.laser)
			return int.MaxValue;
		return base.GetDistance(range, source);
	}

	public override void ActX()
	{
		game.BranchShieldFull(zone);
		Knockout();
		game.ship.DealDamage(zone, 1);
	}
	public override void ActY()
	{
		game.BranchShieldFull(zone);
		Knockout();
		game.ship.DealDamage(zone, 2);
	}
	public override void ActZ()
	{
		game.BranchShieldFull(zone);
		Knockout();
		game.ship.DealDamage(zone, 4);
	}

	private void Knockout()
	{
		if (game.ship.Shields[zone] > 0)
			return;

        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            ref Player p = ref game.ship.Players[i];
            if (p.Position.Zone == zone)
                p.Kill();
        }
	}

	internal override ExThreat Clone(Game game)
	{
		var clone = new	PlasmaticNeedleship();
		clone.CloneThreat(this, game);
		return clone;
	}
}
internal class PolarizedFighter : ExThreat
{
	int laserDamage = 0;
	public PolarizedFighter(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
	{
		health = 4;
		shield = 1;
		speed = 3;
		scoreLose = 4;
		scoreWin = 8;
	}

	public PolarizedFighter() { }

	public override void DealDamage(int damage, int range, ExDmgSource source)
	{
		if (source == ExDmgSource.laser || source == ExDmgSource.plasma)
			laserDamage += damage;
		base.DealDamage(damage, range, source);
	}

	public override bool ProcessDamage()
	{
		damage -= laserDamage / 2;
		laserDamage = 0;
		return base.ProcessDamage();
	}

	public override void ActX()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, 1);
	}
	public override void ActY()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, 2);
	}
	public override void ActZ()
	{
		game.BranchShieldFull(zone);
		game.ship.DealDamage(zone, 4);
	}

	internal override ExThreat Clone(Game game)
	{
		var clone = new	PolarizedFighter();
		clone.CloneThreat(this, game);
		return clone;
	}
}
internal class Siren : InThreat
{
	public Siren(Game game, Trajectory traj, int time) : base(game, traj, time)
	{
		position = Position.TopLeft;
		health = 2;
		speed = 2;
		scoreLose = 8;
		scoreWin = 16;
		vulnerability = InternalDamageType.Android;
		fightBack = true;
	}

	public Siren() { }

	internal override bool DealDamage(Position position, InternalDamageType damageType)
	{
		var hit = base.DealDamage(position, damageType);
		if (hit)
			this.position = Position.BottomRight;

		return hit;
	}

	public override void ActX()
	{
		for(int i = 0; i < game.ship.Players.Length; i++)
        {
            ref Player p0 = ref game.ship.Players[i];
			if (p0.InIntercept)
				continue;

			for(int j = 0; j < game.ship.Players.Length; j++)
			{
				if (i == j)
					continue;
				ref Player p1 = ref game.ship.Players[j];

				if(p0.Position == p1.Position)
					goto Lsurvive;	
			}

			p0.Kill();
			Lsurvive:;
        }
	}
	public override void ActY()
	{
		for(int i = 0; i < game.ship.Players.Length; i++)
        {
            ref Player p0 = ref game.ship.Players[i];
			if (p0.InIntercept)
				continue;

			for(int j = i + 1; j < game.ship.Players.Length; j++)
			{
				ref Player p1 = ref game.ship.Players[j];

				if(p0.Position == p1.Position)
				{
					p0.Kill();
					p1.Kill();
				}
			}
        }
	}
	public override void ActZ()
	{
		for(int i = 0; i < game.ship.Players.Length; i++)
        {
            ref Player p = ref game.ship.Players[i];
			p.Kill();
        }
	}

	internal override InThreat Clone(Game game)
	{
		var clone = new	Siren();
		clone.CloneThreat(this, game);
		return clone;
	}
}


internal class Driller : InThreat
{
	int shield = 1;
	int maxShield = 1;
	public Driller(Game game, Trajectory traj, int time) : base(game, traj, time)
	{
		position = Position.BottomRight;
		health = 2;
		speed = 3;
		scoreLose = 4;
		scoreWin = 8;
		vulnerability = InternalDamageType.Android;
	}

	public Driller() { }

	internal override bool DealDamage(Position position, InternalDamageType damageType)
	{
        if(shield > 0 && damageType == vulnerability && AtPosition(position))
		{
			shield = 0;
			return true;
		}
		
		return base.DealDamage(position, damageType);
	}

	public override bool ProcessTurnEnd()
	{
		shield = maxShield;
		return base.ProcessTurnEnd();
	}

	public override void ActX()
	{
		maxShield = 0;
		shield = 0;
		MoveToMostDamaged();
	}
	public override void ActY()
	{
		MoveToMostDamaged();
	}
	public override void ActZ()
	{
		game.ship.DealDamageIntern(position.Zone, 4);
	}

	private void MoveToMostDamaged()
	{
		int mostDamage = 0;
		int mostDamageIdx = 1;

		for(int i = 0; i < 3; i++)
		{
			if (game.ship.Damage[i] > mostDamage)
			{
				mostDamage = game.ship.Damage[i];
				mostDamageIdx = i;
			}
			else if (game.ship.Damage[i] == mostDamage)
				mostDamageIdx = 1;
		}

		if (position.Zone < mostDamageIdx)
			position = position.GetRight();
		else if (position.Zone > mostDamageIdx)
			position = position.GetLeft();
	}

	internal override InThreat Clone(Game game)
	{
		var clone = new	Driller();
		clone.CloneThreat(this, game);
		return clone;
	}
}
