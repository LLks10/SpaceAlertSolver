namespace SpaceAlertSolver;

#region Common
//ID: 16
class Fregat : ExThreat
{
    public Fregat(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 7;
        shield = 2;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
    }

    public Fregat() { }

    internal override ExThreat Clone(Game game)
    {
        Fregat clone = new Fregat();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 3);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 4);
    }
}
//ID: 17
class GyroFregat : ExThreat
{
    bool hasShield;
    public GyroFregat(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 7;
        shield = 1;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        hasShield = true;
    }

    public GyroFregat() { }

    internal override ExThreat Clone(Game game)
    {
        GyroFregat clone = new GyroFregat();
        clone.CloneThreat(this, game);
        clone.hasShield = hasShield;
        return clone;
    }
    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 3);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 4);
    }
    public override bool ProcessDamage()
    {
        if (!hasShield)
            return base.ProcessDamage();
        else if (damage > 0)
        {
            hasShield = false;
            damage = 0;
            return false;
        }
        return false;
    }
}
//ID: 18
class WarDeck : ExThreat
{
    public WarDeck(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 9;
        shield = 2;
        speed = 1;
        scoreLose = 4;
        scoreWin = 8;
    }

    public WarDeck() { }

    internal override ExThreat Clone(Game game)
    {
        WarDeck clone = new WarDeck();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
        speed++;
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 3);
        shield++;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 5);
    }
}
//ID: 19
class InterStellarOctopus : ExThreat
{
    int maxHealth;
    public InterStellarOctopus(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 8;
        maxHealth = 8;
        shield = 1;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        rocketImmune = true;
    }

    public InterStellarOctopus() { }

    internal override ExThreat Clone(Game game)
    {
        InterStellarOctopus clone = new InterStellarOctopus();
        clone.CloneThreat(this, game);
        clone.maxHealth = maxHealth;
        return clone;
    }
    public override void ActX()
    {
        if (health < maxHealth)
        {
            game.BranchShieldFull(0);
            game.BranchShieldFull(1);
            game.BranchShieldFull(2);
        }

        if(health < maxHealth)
        {
            game.ship.DealDamage(0, 1);
            game.ship.DealDamage(1, 1);
            game.ship.DealDamage(2, 1);
        }
    }
    public override void ActY()
    {
        if (health < maxHealth)
        {
            game.BranchShieldFull(0);
            game.BranchShieldFull(1);
            game.BranchShieldFull(2);
        }

        if (health < maxHealth)
        {
            game.ship.DealDamage(0, 2);
            game.ship.DealDamage(1, 2);
            game.ship.DealDamage(2, 2);
        }
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2*health);
    }
}
//ID: 20
class Maelstorm : ExThreat
{
    int baseShield;
    public Maelstorm(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 8;
        shield = 3;
        baseShield = 3;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
    }

    public Maelstorm() { }

    internal override ExThreat Clone(Game game)
    {
        Maelstorm clone = new Maelstorm();
        clone.CloneThreat(this, game);
        clone.baseShield = baseShield;
        return clone;
    }
    public override void ActX()
    {
        game.ship.Shields[0] = 0;
        game.ship.Shields[1] = 0;
        game.ship.Shields[2] = 0;
    }
    public override void ActY()
    {
        for (int i = 0; i < 3; i++)
        {
            if (zone != i)
                game.BranchShieldFull(i);
        }

        for (int i = 0; i < 3; i++)
        {
            if (zone != i)
                game.ship.DealDamage(i, 2);
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < 3; i++)
        {
            if (zone != i)
                game.BranchShieldFull(i);
        }

        for (int i = 0; i < 3; i++)
        {
            if (zone != i)
                game.ship.DealDamage(i, 3);
        }
    }
    public override void DealDamage(int damage, int range, ExDmgSource source)
    {
        if (source == ExDmgSource.impulse && range >= distanceRange)
            shield = 0;
        base.DealDamage(damage, range, source);
    }
    public override bool ProcessDamage()
    {
        bool v = base.ProcessDamage();
        shield = baseShield;
        return v;
    }
}
//ID: 21
class Asteroid : ExThreat
{
    int revenge;
    public Asteroid(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 9;
        shield = 0;
        speed = 3;
        scoreLose = 4;
        scoreWin = 8;
        rocketImmune = true;
    }

    public Asteroid() { }

    internal override ExThreat Clone(Game game)
    {
        Asteroid clone = new Asteroid();
        clone.CloneThreat(this, game);
        clone.revenge = revenge;
        return clone;
    }
    public override void ActX()
    {
        revenge++;
    }
    public override void ActY()
    {
        revenge++;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, health);
    }
    public override bool ProcessDamage()
    {
        if (health + shield - damage <= 0) // branch if we would die
        {
            game.BranchShieldFull(zone);
        }

        bool v = base.ProcessDamage();
        //Revenge damage if destroyed
        if (v)
            game.ship.DealDamage(zone, revenge*2);
        return v;
    }
}
//ID: 22
class ImpulseSatellite : ExThreat
{
    public ImpulseSatellite(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 4;
        shield = 2;
        speed = 3;
        scoreLose = 4;
        scoreWin = 8;
    }

    public ImpulseSatellite() { }

    internal override ExThreat Clone(Game game)
    {
        ImpulseSatellite clone = new ImpulseSatellite();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActX()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        game.ship.DealDamage(0, 1);
        game.ship.DealDamage(1, 1);
        game.ship.DealDamage(2, 1);
    }
    public override void ActY()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        game.ship.DealDamage(0, 2);
        game.ship.DealDamage(1, 2);
        game.ship.DealDamage(2, 2);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        game.ship.DealDamage(0, 3);
        game.ship.DealDamage(1, 3);
        game.ship.DealDamage(2, 3);
    }
    public override bool ProcessDamage()
    {
        if(distanceRange == 3)
        {
            damage = 0;
            return false;
        }
        return base.ProcessDamage();
    }
}
#endregion

#region Advanced
//ID: 23
class Nemesis : ExThreat
{
    public Nemesis(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 9;
        shield = 1;
        speed = 3;
        scoreLose = 0;
        scoreWin = 12;
    }

    public Nemesis() { }

    internal override ExThreat Clone(Game game)
    {
        Nemesis clone = new Nemesis();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 1);
        health--;
        if (health <= 0)
        {
            beaten = true;
            alive = false;
        }
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
        health -= 2;
        if (health <= 0)
        {
            beaten = true;
            alive = false;
        }
    }
    public override void ActZ()
    {
        game.ship.Damage[0] = 7;
        game.ship.Damage[1] = 7;
        game.ship.Damage[2] = 7;
    }
    public override void DealDamage(int damage, int range, ExDmgSource source)
    {
        if (beaten)
            return;
        base.DealDamage(damage, range, source);
    }
    public override bool ProcessDamage()
    {
        if(damage > shield)
        {
            game.BranchShieldFull(0);
            game.BranchShieldFull(1);
            game.BranchShieldFull(2);

            game.ship.DealDamage(0, 1);
            game.ship.DealDamage(1, 1);
            game.ship.DealDamage(2, 1);
        }
        return base.ProcessDamage();
    }
}
//ID: 24
class NebulaCrab : ExThreat
{
    public NebulaCrab(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 7;
        shield = 2;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
        rocketImmune = true;
    }

    public NebulaCrab() { }

    internal override ExThreat Clone(Game game)
    {
        NebulaCrab clone = new NebulaCrab();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActX()
    {
        shield = 4;
    }
    public override void ActY()
    {
        speed += 2;
        shield = 2;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(2);
        game.ship.DealDamage(0, 5);
        game.ship.DealDamage(2, 5);
    }
}
//ID: 25
class PsionicSatellite : ExThreat
{
    public PsionicSatellite(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 2;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
    }

    public PsionicSatellite() { }

    internal override ExThreat Clone(Game game)
    {
        PsionicSatellite clone = new PsionicSatellite();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActX()
    {
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            ref Player p = ref game.ship.Players[i];
            if (p.Position.Zone == zone)
                p.DelayNext();
        }
    }
    public override void ActY()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        { 
            ref Player p = ref game.ship.Players[i];
            if(p.Position.IsInShip())
                p.DelayNext();
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < game.ship.Players.Length; i++)
        {
            ref Player p = ref game.ship.Players[i];
            if (p.Position.IsInShip())
                p.Kill();
        }
    }
    public override bool ProcessDamage()
    {
        if (distanceRange == 3)
        {
            damage = 0;
            return false;
        }
        return base.ProcessDamage();
    }
}
//ID: 26
class LargeAsteroid : ExThreat
{
    int revenge;
    public LargeAsteroid(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 11;
        shield = 0;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
        rocketImmune = true;
    }

    public LargeAsteroid() { }

    internal override ExThreat Clone(Game game)
    {
        LargeAsteroid clone = new LargeAsteroid();
        clone.CloneThreat(this, game);
        clone.revenge = revenge;
        return clone;
    }
    public override void ActX()
    {
        revenge++;
    }
    public override void ActY()
    {
        revenge++;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, health);
    }
    public override bool ProcessDamage()
    {
        if (health + shield - damage <= 0)
        {
            game.BranchShieldFull(zone);
        }

        bool v = base.ProcessDamage();
        //Revenge damage if destroyed
        if (v)
            game.ship.DealDamage(zone, revenge * 3);
        return v;
    }
}

//ID: 27
class Moloch : ExThreat
{
    bool increaseShield = false;
    public Moloch(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 10;
        shield = 3;
        speed = 1;
        scoreLose = 6;
        scoreWin = 12;
    }

    public Moloch() { }

    internal override ExThreat Clone(Game game)
    {
        Moloch clone = new Moloch();
        clone.CloneThreat(this, game);
        clone.increaseShield = increaseShield;
        return clone;
    }
    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
        speed += 2;
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 3);
        speed += 2;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 7);
    }
    public override int GetDistance(int range, ExDmgSource source)
    {
        //Priority target for rocket
        if (source == ExDmgSource.rocket)
            return -100;
        return base.GetDistance(range, source);
    }
    public override void DealDamage(int damage, int range, ExDmgSource source)
    {
        if(source == ExDmgSource.rocket)
        {
            increaseShield = true;
            this.damage += damage;
        }
        else
            base.DealDamage(damage, range, source);
    }
    public override bool ProcessDamage()
    {
        bool r = base.ProcessDamage();
        if (increaseShield)
        {
            shield++;
            increaseShield = false;
        }
        return r;
    }
}

//ID: 28
class Behemoth : ExThreat
{
    int maxHealth;
    public Behemoth(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 7;
        maxHealth = health;
        shield = 4;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
    }

    public Behemoth() { }

    internal override ExThreat Clone(Game game)
    {
        Behemoth clone = new Behemoth();
        clone.CloneThreat(this, game);
        clone.maxHealth = maxHealth;
        return clone;
    }
    public override void ActX()
    {
        if (maxHealth - health < 2)
        {
            game.BranchShieldFull(zone);
            game.ship.DealDamage(zone, 2);
        }
    }
    public override void ActY()
    {
        if (maxHealth - health < 3)
        {
            game.BranchShieldFull(zone);
            game.ship.DealDamage(zone, 3);
        }
    }
    public override void ActZ()
    {
        if (maxHealth - health < 6)
        {
            game.BranchShieldFull(zone);
            game.ship.DealDamage(zone, 6);
        }
    }
    public override void DealDamage(int damage, int range, ExDmgSource source)
    {
        //Valid cause interceptor performs range check
        if (source == ExDmgSource.intercept && damage == 3)
        {
            damage = 9;
            //Kill player
            Player[] ps = game.ship.Players;
            for(int i = 0; i < ps.Length; i++)
            {
                if (ps[i].InIntercept)
                {
                    ps[i].Kill();
                    break;
                }
            }
        }

        base.DealDamage(damage, range, source);
    }
}
#endregion
