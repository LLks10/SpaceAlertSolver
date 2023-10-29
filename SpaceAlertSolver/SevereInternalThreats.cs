namespace SpaceAlertSolver;

//count: 5

class CommandosRed : InThreat
{
    public CommandosRed(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 2;
        position = 3;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InDmgSource.android;
        fightBack = true;
    }

    public CommandosRed() { }

    public override InThreat Clone(Ship ship)
    {
        CommandosRed clone = new CommandosRed();
        clone.CloneThreat(this, ship);
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
            ship.DealDamageIntern(z, 2);
        }            
    }
    public override void ActZ()
    {
        int z = position % 3;
        ship.DealDamageIntern(z, 4);
        for(int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position == position)
                ship.players[i].Kill();
        }
    }
}

class CommandosBlue : InThreat
{
    public CommandosBlue(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 2;
        position = 2;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InDmgSource.android;
        fightBack = true;
    }

    public CommandosBlue() { }

    public override InThreat Clone(Ship ship)
    {
        CommandosBlue clone = new CommandosBlue();
        clone.CloneThreat(this, ship);
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
            ship.DealDamageIntern(z, 2);
        }
    }
    public override void ActZ()
    {
        int z = position % 3;
        ship.DealDamageIntern(z, 4);
        for (int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position == position)
                ship.players[i].Kill();
        }
    }
}

class Alien : InThreat
{
    public Alien(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 2;
        position = 4;
        speed = 2;
        scoreLose = 0;
        scoreWin = 8;
        vulnerability = InDmgSource.android;
    }

    public Alien() { }

    public override InThreat Clone(Ship ship)
    {
        Alien clone = new Alien();
        clone.CloneThreat(this, ship);
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
        for(int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position == position)
                c++;
        }
        ship.DealDamageIntern(position % 3, c);
    }
    public override void ActZ()
    {
        ship.damage[0] = 7;
        ship.damage[1] = 7;
        ship.damage[2] = 7;
    }
}

class Eliminator : InThreat
{
    public Eliminator(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 2;
        position = 2;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
        vulnerability = InDmgSource.android;
        fightBack = true;
    }

    public Eliminator() { }

    public override InThreat Clone(Ship ship)
    {
        Eliminator clone = new Eliminator();
        clone.CloneThreat(this, ship);
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
        ship.DealDamageIntern(position % 3, 3);
    }

    private void KillAll()
    {
        int z = position % 3;
        for(int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position % 3 == z && ship.players[i].position < 6 && (ship.players[i].team == null || ship.players[i].team.alive == false))
                ship.players[i].Kill();
        }
    }
}

class SearchRobot : InThreat
{
    public SearchRobot(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 2;
        position = 1;
        speed = 2;
        scoreLose = 6;
        scoreWin = 15;
        vulnerability = InDmgSource.android;
    }

    public SearchRobot() { }

    public override InThreat Clone(Ship ship)
    {
        SearchRobot clone = new SearchRobot();
        clone.CloneThreat(this, ship);
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
        ship.DealDamageIntern(position % 3, 5);
        for(int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position == position)
                ship.players[i].Kill();
        }
    }

    public override bool DealDamage(int position, InDmgSource source)
    {
        if (source == vulnerability && AtPosition(position))
        {
            health--;
            if (health <= 0)
            {
                //Kill killing player
                int highestAct = -1;
                int score = int.MinValue;
                //Find player that did most recent action
                for(int i = 0; i < ship.players.Length; i++)
                {
                    if(ship.players[i].lastAction >= score)
                    {
                        highestAct = i;
                        score = ship.players[i].lastAction;
                    }
                }
                ship.players[highestAct].Kill();

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
        Player[] ps = ship.players;
        //Count players and choose best station
        for(int i = 0; i < nearbyStation.Length; i++)
        {
            int c = 0;
            //Count players
            for(int j = 0; j < ps.Length; j++)
            {
                if (ps[j].position == nearbyStation[i])
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
    public AtomicBomb(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 1;
        position = 4;
        ship.CDefect[4]++;
        speed = 4;
        scoreLose = 0;
        scoreWin = 12;
        vulnerability = InDmgSource.C;
    }

    public AtomicBomb() { }

    public override InThreat Clone(Ship ship)
    {
        AtomicBomb clone = new AtomicBomb();
        clone.CloneThreat(this, ship);
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
        ship.damage[0] = 7;
        ship.damage[1] = 7;
        ship.damage[2] = 7;
    }
    public override bool DealDamage(int position, InDmgSource source)
    {
        if (source == vulnerability && AtPosition(position))
        {
            damage++;
            if(damage >= 3)
            {
                health--;
                beaten = true;
                alive = false;
                ship.CDefect[4]--;
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
    public RebelliousRobots(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 4;
        ship.CDefect[2]++;
        ship.CDefect[3]++;
        speed = 2;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InDmgSource.C;
    }

    public RebelliousRobots() { }

    public override InThreat Clone(Ship ship)
    {
        RebelliousRobots clone = new RebelliousRobots();
        clone.CloneThreat(this, ship);
        clone.tookExtraDamage = tookExtraDamage;
        clone.hits = hits.ToArray();
        return clone;
    }
    public override void OnClear()
    {
        ship.CDefect[2]--;
        ship.CDefect[3]--;
    }
    public override void ActX()
    {
        for(int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].team != null && ship.players[i].team.alive)
                ship.players[i].Kill();
        }
    }
    public override void ActY()
    {
        for (int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position == 2 || ship.players[i].position == 3)
                ship.players[i].Kill();
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < ship.players.Length; i++)
        {
            if (ship.players[i].position != 1)
                ship.players[i].Kill();
        }
    }
    public override bool DealDamage(int position, InDmgSource source)
    {
        if (source == vulnerability)
        {
            //Check if at any of the two positions
            if(position == 2 || position == 3)
            {
                health--;
                //Bonus damage if both stations activated
                hits[position - 2] = true;
                if (!tookExtraDamage)
                {
                    if(hits[0] && hits[1])
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
        return false;
    }
}

class SwitchedCables : InThreat
{
    public SwitchedCables(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 4;
        position = 1;
        ship.BDefect[1]++;
        speed = 3;
        scoreLose = 4;
        scoreWin = 8;
        vulnerability = InDmgSource.B;
    }

    public SwitchedCables() { }

    public override InThreat Clone(Ship ship)
    {
        SwitchedCables clone = new SwitchedCables();
        clone.CloneThreat(this, ship);
        return clone;
    }

    public override void OnClear()
    {
        ship.BDefect[1]--;
    }

    public override void ActX()
    {
        ship.game.BranchReactorFull(1);
        ship.game.BranchConditional(1, Defects.shield);
        ship.shields[1] += ship.reactors[1];
        ship.reactors[1] = 0;
        int excess = ship.shields[1] - ship.shieldsCap[1];
        if(excess > 0)
        {
            ship.shields[1] = ship.shieldsCap[1];
            ship.DealDamageIntern(1, excess);
        }

    }
    public override void ActY()
    {
        ship.game.BranchShieldFull(1);
        ship.DealDamageIntern(1, ship.shields[1]);
        ship.shields[1] = 0;
    }
    public override void ActZ()
    {
        ship.game.BranchReactorFull(0);
        ship.game.BranchReactorFull(1);
        ship.game.BranchReactorFull(2);

        for (int i = 0; i < 3; i++)
        {
            ship.DealDamageIntern(i, ship.reactors[i]);
            ship.reactors[i] = 0;
        }
    }
}

class OverstrainedEnergyNet : InThreat
{
    bool tookExtraDamage;
    bool[] hits = new bool[3];
    public OverstrainedEnergyNet(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 7;
        ship.BDefect[3]++;
        ship.BDefect[4]++;
        ship.BDefect[5]++;
        speed = 3;
        scoreLose = 6;
        scoreWin = 12;
        vulnerability = InDmgSource.B;
    }

    public OverstrainedEnergyNet() { }

    public override InThreat Clone(Ship ship)
    {
        OverstrainedEnergyNet clone = new OverstrainedEnergyNet();
        clone.CloneThreat(this, ship);
        clone.tookExtraDamage = tookExtraDamage;
        clone.hits = hits.ToArray();
        return clone;
    }
    public override void OnClear()
    {
        ship.BDefect[3]--;
        ship.BDefect[4]--;
        ship.BDefect[5]--;
    }
    public override void ActX()
    {
        ship.game.BranchReactorFull(1);
        ship.reactors[1] = Math.Max(0, ship.reactors[1] - 2);
    }
    public override void ActY()
    {
        ship.game.BranchReactorFull(0);
        ship.game.BranchReactorFull(1);
        ship.game.BranchReactorFull(2);
        for (int i = 0; i < 3; i++)
            ship.reactors[i] = Math.Max(0, ship.reactors[i] - 1);
    }
    public override void ActZ()
    {
        ship.DealDamageIntern(0, 3);
        ship.DealDamageIntern(1, 3);
        ship.DealDamageIntern(2, 3);
    }
    public override bool DealDamage(int position, InDmgSource source)
    {
        if (source == vulnerability)
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
    public Fissure(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 2;
        position = 6;
        ship.CDefect[6]++;
        speed = 2;
        scoreLose = 0;
        scoreWin = 8;
        vulnerability = InDmgSource.C;
    }

    public Fissure() { }

    public override InThreat Clone(Ship ship)
    {
        Fissure clone = new Fissure();
        clone.CloneThreat(this, ship);
        return clone;
    }
    public override void OnClear()
    {
        ship.CDefect[6]--;
        ship.fissured[0] = false;
        ship.fissured[1] = false;
        ship.fissured[2] = false;
    }
    public override void ActX()
    {
        ship.fissured[0] = true;
    }
    public override void ActY()
    {
        ship.fissured[0] = true;
        ship.fissured[1] = true;
        ship.fissured[2] = true;
    }
    public override void ActZ()
    {
        ship.damage[0] = 7;
        ship.damage[1] = 7;
        ship.damage[2] = 7;
    }
}

class Infection : InThreat
{
    bool[] isActive = new bool[] {true,false,true,true,false,true,false };
    public Infection(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
    {
        health = 3;
        speed = 2;
        scoreLose = 6;
        scoreWin = 12;
        vulnerability = InDmgSource.android;
    }

    public Infection() { }

    public override InThreat Clone(Ship ship)
    {
        Infection clone = new Infection();
        clone.CloneThreat(this, ship);
        clone.isActive = isActive.ToArray();
        return clone;
    }

    public override void ActX()
    {
        for(int i = 0; i < ship.players.Length; i++)
        {
            if (isActive[ship.players[i].position])
                ship.players[i].Delay(ship.players[i].lastAction + 1);
        }
    }
    public override void ActY()
    {
        for (int i = 0; i < 6; i++)
        {
            if (isActive[i])
                ship.DealDamageIntern(i % 3, 1);
        }
    }
    public override void ActZ()
    {
        for (int i = 0; i < ship.players.Length; i++)
        {
            if (isActive[ship.players[i].position])
                ship.players[i].Kill();
        }
        for(int i = 0; i < 6; i++)
        {
            if (isActive[i])
                ship.stationStatus[i] |= 2;
        }
    }
    public override bool DealDamage(int position, InDmgSource source)
    {
        if (source == vulnerability && isActive[position])
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
