namespace SpaceAlertSolver;

#region Normals
//ID: 0
class ArmoredCatcher : ExThreat
{
    int maxHealth;
    public ArmoredCatcher(Game game, Trajectory traj, int zone, int time) : base(game,traj,zone,time)
    {
        health = 4;
        maxHealth = 4;
        shield = 3;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
    }

    public ArmoredCatcher() { }

    internal override ExThreat Clone(Game game)
    {
        ArmoredCatcher clone = new ArmoredCatcher();
        clone.CloneThreat(this, game);
        clone.maxHealth = maxHealth;
        return clone;
    }

    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 1);
    }
    public override void ActY()
    {
        health = Math.Min(health + 1, maxHealth);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 4);
    }
}

//ID: 1
class Amoebe : ExThreat
{
    int maxHealth;
    public Amoebe(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 8;
        maxHealth = 8;
        shield = 0;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
        rocketImmune = true;
    }

    public Amoebe() { }

    internal override ExThreat Clone(Game game)
    {
        Amoebe clone = new Amoebe();
        clone.CloneThreat(this, game);
        clone.maxHealth = maxHealth;
        return clone;
    }

    public override void ActX()
    {
        health = Math.Min(health + 2, maxHealth);
    }
    public override void ActY()
    {
        health = Math.Min(health + 2, maxHealth);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 5);
    }
}

//ID: 2
class Battleship : ExThreat
{
    public Battleship(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 2;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
    }

    public Battleship() { }

    internal override ExThreat Clone(Game game)
    {
        Battleship clone = new Battleship();
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
        game.ship.DealDamage(zone, 2);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 3);
    }
}

//ID: 3
class Hunter : ExThreat
{
    public Hunter(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 4;
        shield = 2;
        speed = 3;
        scoreLose = 2;
        scoreWin = 4;
    }

    public Hunter() { }

    internal override ExThreat Clone(Game game)
    {
        Hunter clone = new Hunter();
        clone.CloneThreat(this, game);
        return clone;
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
        game.ship.DealDamage(zone, 3);
    }
}

//ID: 4
class GyroHunter : ExThreat
{
    bool hasShield;
    public GyroHunter(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 4;
        shield = 1;
        speed = 3;
        scoreLose = 2;
        scoreWin = 4;
        hasShield = true;
    }

    public GyroHunter() { }

    internal override ExThreat Clone(Game game)
    {
        GyroHunter clone = new GyroHunter();
        clone.CloneThreat(this, game);
        clone.hasShield = hasShield;
        return clone;
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
        game.ship.DealDamage(zone, 2);
    }
    public override bool ProcessDamage()
    {
        if(!hasShield)
            return base.ProcessDamage();
        else if(damage > 0)
        {
            hasShield = false;
            damage = 0;
            return false;
        }
        return false;
    }
}

//ID: 5
class EnergyCloud : ExThreat
{
    int baseShield;
    public EnergyCloud(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 3;
        baseShield = 3;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
    }

    public EnergyCloud() { }

    internal override ExThreat Clone(Game game)
    {
        EnergyCloud clone = new EnergyCloud();
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

        for(int i = 0; i < 3; i++)
        {
            if (zone != i)
                game.ship.DealDamage(i, 1);
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
                game.ship.DealDamage(i, 2);
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

//ID: 6
class Meteorite : ExThreat
{
    public Meteorite(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 0;
        speed = 5;
        scoreLose = 2;
        scoreWin = 4;
        rocketImmune = true;
    }

    public Meteorite() { }

    internal override ExThreat Clone(Game game)
    {
        Meteorite clone = new Meteorite();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, health);
    }
}

//ID: 7
class ImpulseBall : ExThreat
{
    public ImpulseBall(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 1;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
    }

    public ImpulseBall() { }

    internal override ExThreat Clone(Game game)
    {
        ImpulseBall clone = new ImpulseBall();
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
        game.ship.DealDamage(0, 1);
        game.ship.DealDamage(1, 1);
        game.ship.DealDamage(2, 1);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        game.ship.DealDamage(0, 2);
        game.ship.DealDamage(1, 2);
        game.ship.DealDamage(2, 2);
    }
}

//ID: 8
class SpaceCruiser : ExThreat
{
    public SpaceCruiser(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 2;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
    }

    public SpaceCruiser() { }

    internal override ExThreat Clone(Game game)
    {
        SpaceCruiser clone = new SpaceCruiser();
        clone.CloneThreat(this, game);
        return clone;
    }
    public override void ActX()
    {
        game.BranchShieldFull(zone);
        if (game.ship.Shields[zone] == 0)
            game.ship.DealDamage(zone, 2);
        else
            game.ship.DealDamage(zone, 1);
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);

        int dmg = 2;
        int bonus = 2 - game.ship.Shields[zone];
        if (bonus > 0)
            dmg += bonus;

        game.ship.DealDamage(zone, dmg);
    }
    public override void ActZ()
    {
        ActY();
    }
}

//ID: 9
class StealthHunter : ExThreat
{
    bool visible = false;
    public StealthHunter(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 4;
        shield = 2;
        speed = 3;
        scoreLose = 2;
        scoreWin = 4;
    }

    public StealthHunter() { }

    internal override ExThreat Clone(Game game)
    {
        StealthHunter clone = new StealthHunter();
        clone.CloneThreat(this, game);
        clone.visible = visible;
        return clone;
    }
    public override void ActX()
    {
        visible = true;
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
    }
    public override void ActZ()
    {
        ActY();
    }
    public override int GetDistance(int range, ExDmgSource source)
    {
        if (!visible)
            return 100;
        return base.GetDistance(range, source);
    }
    public override bool ProcessDamage()
    {
        if (!visible)
        {
            damage = 0;
            return false;
        }
        return base.ProcessDamage();
    }
}
#endregion

#region Advanced
//ID: 10
class JellyFish : ExThreat
{
    int baseHealth;
    public JellyFish(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 13;
        baseHealth = health;
        shield = -2;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        rocketImmune = true;
    }

    public JellyFish() { }

    internal override ExThreat Clone(Game game)
    {
        JellyFish clone = new JellyFish();
        clone.CloneThreat(this, game);
        clone.baseHealth = baseHealth;
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
        int damage = baseHealth - health;
        health = Math.Min(baseHealth, health + damage / 2);
    }
    public override void ActY()
    {
        ActX();
    }
    public override void ActZ()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        game.ship.DealDamage(0, 2);
        game.ship.DealDamage(1, 2);
        game.ship.DealDamage(2, 2);
    }
    public override bool ProcessDamage()
    {
        //Prevent -2 shield dealing damage
        if(damage > 0 )
            return base.ProcessDamage();
        return false;
    }
}

//ID: 11
class SmallAsteroid : ExThreat
{
    int revenge;
    public SmallAsteroid(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 7;
        shield = 0;
        speed = 4;
        scoreLose = 3;
        scoreWin = 6;
        rocketImmune = true;
    }

    public SmallAsteroid() { }

    internal override ExThreat Clone(Game game)
    {
        SmallAsteroid clone = new SmallAsteroid();
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
        ActX();
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, health);
    }
    public override bool ProcessDamage()
    {
        if (health + shield - damage <= 0) // if we will die, branch
        {
            game.BranchShieldFull(zone);
        }

        health = health - Math.Max(0, (damage - shield));
        damage = 0;

        if (health <= 0)
        {
            //Deal revenge damage
            game.ship.DealDamage(zone, revenge);
            alive = false;
            beaten = true;
            return true;
        }
        return false;
    }
}

//ID: 12
class Kamikaze : ExThreat
{
    public Kamikaze(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 5;
        shield = 2;
        speed = 4;
        scoreLose = 3;
        scoreWin = 6;
    }

    public Kamikaze() { }

    internal override ExThreat Clone(Game game)
    {
        Kamikaze clone = new Kamikaze();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        speed += 1;
        shield = 1;
    }
    public override void ActY()
    {
        speed += 1;
        shield = 0;
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 6);
    }
}

//ID: 13
class Swarm : ExThreat
{
    public Swarm(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 3;
        shield = 0;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        rocketImmune = true;
    }

    public Swarm() { }

    internal override ExThreat Clone(Game game)
    {
        Swarm clone = new Swarm();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 1);
    }
    public override void ActY()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        for (int i = 0; i < 3; i++)
        {
            if (i == zone)
                game.ship.DealDamage(i, 2);
            else
                game.ship.DealDamage(i, 1);
        }
    }
    public override void ActZ()
    {
        game.BranchShieldFull(0);
        game.BranchShieldFull(1);
        game.BranchShieldFull(2);
        for (int i = 0; i < 3; i++)
        {
            if (i == zone)
                game.ship.DealDamage(i, 3);
            else
                game.ship.DealDamage(i, 2);
        }
    }
    public override bool ProcessDamage()
    {
        damage = Math.Min(damage, 1);
        return base.ProcessDamage();
    }
}
//ID: 14
class GhostHunter : ExThreat
{
    bool visible = false;
    public GhostHunter(Game game, Trajectory traj, int zone,int time) : base(game, traj, zone, time)
    {
        health = 3;
        shield = 3;
        speed = 3;
        scoreLose = 3;
        scoreWin = 6;
    }

    public GhostHunter() { }

    internal override ExThreat Clone(Game game)
    {
        GhostHunter clone = new GhostHunter();
        clone.CloneThreat(this, game);
        clone.visible = visible;
        return clone;
    }
    public override void ActX()
    {
        visible = true;
    }
    public override void ActY()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 2);
    }
    public override void ActZ()
    {
        game.BranchShieldFull(zone);
        game.ship.DealDamage(zone, 3);
    }
    public override int GetDistance(int range, ExDmgSource source)
    {
        if (!visible)
            return 100;
        return base.GetDistance(range, source);
    }
    public override void DealDamage(int damage, int range, ExDmgSource source)
    {
        if (!visible)
            return;
        if (source == ExDmgSource.rocket)
            return;

        base.DealDamage(damage, range, source);
    }
}
//ID: 15
class Scout : ExThreat
{
    int act_y_i = 0;

    public Scout(Game game, Trajectory traj, int zone, int time) : base(game, traj, zone, time)
    {
        health = 3;
        shield = 1;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
    }

    public Scout() { }

    internal override ExThreat Clone(Game game)
    {
        Scout clone = new Scout();
        clone.CloneThreat(this, game);
        clone.act_y_i = act_y_i;
        return clone;
    }
    public override void ActX()
    {
        game.ScoutBonus = 1;
    }
    public override void ActY()
    {
        List<ExThreat> trts = game.exThreats;

        while (act_y_i < trts.Count)
        {
            if (trts[act_y_i] is Scout || trts[act_y_i].beaten)
            {
                act_y_i++;
                continue;
            }
            trts[act_y_i].Move(1);
            act_y_i++;
        }

        act_y_i = 0;
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(zone, 3);
    }
    public override int GetDistance(int range, ExDmgSource source)
    {
        if (source == ExDmgSource.laser)
            return 100;
        return base.GetDistance(range, source);
    }
    public override void DealDamage(int damage, int range, ExDmgSource source)
    {
        if (source == ExDmgSource.laser)
            return;

        base.DealDamage(damage, range, source);
    }
    public override bool ProcessDamage()
    {
        bool r = base.ProcessDamage();
        if (r)
            game.ScoutBonus = 0;
        return r;
    }
}
#endregion
