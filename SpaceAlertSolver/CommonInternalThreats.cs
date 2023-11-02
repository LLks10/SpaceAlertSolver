namespace SpaceAlertSolver;

#region Normals
//ID: 0
class SaboteurRed : InThreat
{
    public SaboteurRed(Game game, Trajectory traj, int time) : base(game,traj,time)
    {
        health = 1;
        position = 4;
        speed = 4;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.Android;
    }

    public SaboteurRed() { }

    internal override InThreat Clone(Game game)
    {
        SaboteurRed clone = new SaboteurRed();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        if (position > 3)
            position--;
    }
    public override void ActY()
    {
        int z = position % 3;
        if (game.ship.Reactors[z] == 0)
            game.ship.DealDamageIntern(z, 1);
        else
        {
            game.BranchReactorFull(z);
            game.ship.Reactors[z]--;
        }
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 2);
    }
}
//ID: 1
class SaboteurBlue : InThreat
{
    public SaboteurBlue(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 1;
        position = 4;
        speed = 4;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.Android;
    }

    public SaboteurBlue() { }

    internal override InThreat Clone(Game game)
    {
        SaboteurBlue clone = new SaboteurBlue();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        if (position < 5)
            position++;
    }
    public override void ActY()
    {
        int z = position % 3;
        if (game.ship.Reactors[z] == 0)
            game.ship.DealDamageIntern(z, 1);
        else
        {
            game.BranchReactorFull(z);
            game.ship.Reactors[z]--;
        }
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 2);
    }
}
//ID: 2
class SkirmisherRed : InThreat
{
    public SkirmisherRed(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 1;
        position = 0;
        speed = 3;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public SkirmisherRed() { }

    internal override InThreat Clone(Game game)
    {
        SkirmisherRed clone = new SkirmisherRed();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        if (position != 2 && position != 5)
            position++;
    }
    public override void ActY()
    {
        if (position < 3)
            position += 3;
        else
            position -= 3;
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 3);
    }
}
//ID: 3
class SkirmisherBlue : InThreat
{
    public SkirmisherBlue(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 1;
        position = 2;
        speed = 3;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public SkirmisherBlue() { }

    internal override InThreat Clone(Game game)
    {
        SkirmisherBlue clone = new SkirmisherBlue();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void ActX()
    {
        if (position != 0 && position != 3)
            position--;
    }
    public override void ActY()
    {
        if (position < 3)
            position += 3;
        else
            position -= 3;
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 3);
    }
}
//ID: 4
class SoldiersRed : InThreat
{
    public SoldiersRed(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 0;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public SoldiersRed() { }

    internal override InThreat Clone(Game game)
    {
        SoldiersRed clone = new SoldiersRed();
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
        if (position != 2 && position != 5)
            position++;
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 4);
    }
}
//ID: 5
class SoldiersBlue : InThreat
{
    public SoldiersBlue(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        position = 5;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        vulnerability = InternalDamageType.Android;
        fightBack = true;
    }

    public SoldiersBlue() { }

    internal override InThreat Clone(Game game)
    {
        SoldiersBlue clone = new SoldiersBlue();
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
        if (position != 0 && position != 3)
            position--;
    }
    public override void ActZ()
    {
        int z = position % 3;
        game.ship.DealDamageIntern(z, 4);
    }
}
//ID: 6
class Virus : InThreat
{
    public Virus(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 3;
        position = 1;
        game.ship.CDefect[1]++;
        speed = 3;
        scoreLose = 3;
        scoreWin = 6;
        vulnerability = InternalDamageType.C;
    }

    public Virus() { }

    internal override InThreat Clone(Game game)
    {
        Virus clone = new Virus();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void OnClear()
    {
        game.ship.CDefect[1]--;
    }

    public override void ActX()
    {
        game.BranchReactorFull(0);
        game.BranchReactorFull(1);
        game.BranchReactorFull(2);

        if (game.ship.Reactors[0] > 0)
            game.ship.Reactors[0]--;
        if (game.ship.Reactors[1] > 0)
            game.ship.Reactors[1]--;
        if (game.ship.Reactors[2] > 0)
            game.ship.Reactors[2]--;

    }
    public override void ActY()
    {
        for(int i = 0; i < game.ship.Players.Length; i++)
        {
            if (game.ship.Players[i].Position < 6)
                game.ship.Players[i].DelayNext();
        }
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(0, 1);
        game.ship.DealDamageIntern(1, 1);
        game.ship.DealDamageIntern(2, 1);
    }
}
//ID: 7
class HackedShieldsRed : InThreat
{
    public HackedShieldsRed(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 3;
        position = 0;
        game.ship.BDefect[0]++;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.B;
    }

    public HackedShieldsRed() { }

    internal override InThreat Clone(Game game)
    {
        HackedShieldsRed clone = new HackedShieldsRed();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void OnClear()
    {
        game.ship.BDefect[0]--;
    }

    public override void ActX()
    {
        game.ship.Shields[0] = 0;

    }
    public override void ActY()
    {
        game.ship.Reactors[0] = 0;
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(0, 2);
    }
}
//ID: 8
class HackedShieldsBlue : InThreat
{
    public HackedShieldsBlue(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 3;
        position = 2;
        game.ship.BDefect[2]++;
        speed = 2;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.B;
    }

    public HackedShieldsBlue() { }

    internal override InThreat Clone(Game game)
    {
        HackedShieldsBlue clone = new HackedShieldsBlue();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void OnClear()
    {
        game.ship.BDefect[2]--;
    }

    public override void ActX()
    {
        game.ship.Shields[2] = 0;

    }
    public override void ActY()
    {
        game.ship.Reactors[2] = 0;
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(2, 2);
    }
}
//ID: 9
class OverheatedReactor : InThreat
{
    public OverheatedReactor(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 3;
        position = 4;
        game.ship.BDefect[4]++;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        vulnerability = InternalDamageType.B;
    }

    public OverheatedReactor() { }

    internal override InThreat Clone(Game game)
    {
        OverheatedReactor clone = new OverheatedReactor();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void OnClear()
    {
        game.ship.BDefect[4]--;
    }

    public override void ActX()
    {
        game.BranchReactorFull(1);
        game.ship.DealDamageIntern(1, game.ship.Reactors[1]);
    }
    public override void ActY()
    {
        if (game.ship.CapsulesLeft > 0)
            game.ship.CapsulesLeft--;
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(1, 2);
    }
    internal override bool DealDamage(int position, InternalDamageType damageType)
    {
        bool r = base.DealDamage(position, damageType);

        if (r)
        {
            for(int i = 0; i < game.ship.Players.Length; i++)
            {
                if (game.ship.Players[i].Position == 3 || game.ship.Players[i].Position == 5)
                    game.ship.Players[i].Kill();
            }
        }

        return r;
    }
}
//ID: 10
class UnstableWarheads : InThreat
{
    public UnstableWarheads(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = game.ship.RocketsLeft;
        position = 5;
        speed = 3;
        scoreLose = 2;
        scoreWin = 4;
        vulnerability = InternalDamageType.C;
        if(health > 0)
            game.ship.CDefect[5]++;
        else
        {
            beaten = true;
            alive = false;
        }
    }

    public UnstableWarheads() { }

    internal override InThreat Clone(Game game)
    {
        UnstableWarheads clone = new UnstableWarheads();
        clone.CloneThreat(this, game);
        return clone;
    }

    public override void OnClear()
    {
        game.ship.CDefect[5]--;
    }
    public override void ActZ()
    {
        game.ship.DealDamageIntern(2, game.ship.RocketsLeft*3);
    }
}

//ID: 11
class SlimeBlue : InThreat
{
    bool[] positions;
    int[] healths;
    public SlimeBlue(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        vulnerability = InternalDamageType.Android;

        // Apply station based health and position
        healths = new int[] { 0, 0, 0, 0, 0, 2, 0 };
        positions = new bool[] { false, false, false, false, false, true, false };
        // Apply delay to ship
        game.ship.StationStatus[5] |= 1;
    }

    public SlimeBlue() { }
    
    internal override InThreat Clone(Game game)
    {
        SlimeBlue clone = new SlimeBlue();
        clone.CloneThreat(this, game);
        clone.positions = positions.ToArray();
        clone.healths = healths.ToArray();
        return clone;
    }

    // Destroy a rocket
    public override void ActX()
    {
        if (game.ship.RocketsLeft > 0)
        {
            game.ship.RocketsLeft--;
            // Check if Cdefect is active
            if (game.ship.CDefect[5] > 0)
            {
                // Search for unstable warheads and deal damage
                for (int i = 0; i < game.inThreats.Count; i++)
                {
                    if (game.inThreats[i] is UnstableWarheads)
                    {
                        game.inThreats[i].DealDamage(i, InternalDamageType.C);
                        break;
                    }
                }
            }
        }
    }

    public override void ActY()
    {
        // Check if slime exists in lower white
        if (positions[4])
        {
            // Check if red lower area doesn't already have slime effect
            if((game.ship.StationStatus[3] & 1) != 1)
            {
                // Spread
                positions[3] = true;
                healths[3] = 1;
                health++;
                game.ship.StationStatus[3] |= 1;
            }
        }
        // Check if slime exists in lower blue
        if (positions[5])
        {
            // Check if white lower area doesn't already have slime effect
            if ((game.ship.StationStatus[4] & 1) != 1)
            {
                // Spread
                positions[4] = true;
                healths[4] = 1;
                health++;
                game.ship.StationStatus[4] |= 1;
            }
        }
    }
    public override void ActZ()
    {
        if (positions[3])
            game.ship.DealDamageIntern(0, 2);
        if (positions[4])
            game.ship.DealDamageIntern(1, 2);
        if (positions[5])
            game.ship.DealDamageIntern(2, 2);
    }

    internal override bool DealDamage(int position, InternalDamageType damageType)
    {
        if (damageType == vulnerability && AtPosition(position))
        {
            //Local damage
            healths[position]--;
            if (healths[position] <= 0)
            {
                // Remove slime from position
                game.ship.StationStatus[position] &= ~1;
                positions[position] = false;
            }

            //Full damage
            health--;
            if (health <= 0)
            {
                alive = false;
                beaten = true;
            }
            return true;
        }
        return false;
    }

    public override bool AtPosition(int position)
    {
        return positions[position];
    }
}

//ID: 12
class SlimeRed : InThreat
{
    bool[] positions;
    int[] healths;
    public SlimeRed(Game game, Trajectory traj, int time) : base(game, traj, time)
    {
        health = 2;
        speed = 2;
        scoreLose = 3;
        scoreWin = 6;
        vulnerability = InternalDamageType.Android;

        // Apply station based health and position
        healths = new int[] { 0, 0, 0, 2, 0, 0, 0 };
        positions = new bool[] { false, false, false, true, false, false, false };
        // Apply delay to ship
        game.ship.StationStatus[3] |= 1;
    }

    public SlimeRed() { }

    internal override InThreat Clone(Game game)
    {
        SlimeRed clone = new SlimeRed();
        clone.CloneThreat(this, game);
        clone.positions = positions.ToArray();
        clone.healths = healths.ToArray();
        return clone;
    }

    // Disable inactive androids
    public override void ActX()
    {
        if (game.ship.AndroidBottomLeft == AndroidState.Alive)
            game.ship.AndroidBottomLeft = AndroidState.Disabled;
    }
    
    public override void ActY()
    {
        // Check if slime exists in lower white
        if (positions[4])
        {
            // Check if blue lower area doesn't already have slime effect
            if ((game.ship.StationStatus[5] & 1) != 1)
            {
                // Spread
                positions[5] = true;
                healths[5] = 1;
                health++;
                game.ship.StationStatus[5] |= 1;
            }
        }
        // Check if slime exists in lower red
        if (positions[3])
        {
            // Check if white lower area doesn't already have slime effect
            if ((game.ship.StationStatus[4] & 1) != 1)
            {
                // Spread
                positions[4] = true;
                healths[4] = 1;
                health++;
                game.ship.StationStatus[4] |= 1;
            }
        }
    }
    public override void ActZ()
    {
        if (positions[3])
            game.ship.DealDamageIntern(0, 2);
        if (positions[4])
            game.ship.DealDamageIntern(1, 2);
        if (positions[5])
            game.ship.DealDamageIntern(2, 2);
    }

    internal override bool DealDamage(int position, InternalDamageType damageType)
    {
        if (damageType == vulnerability && AtPosition(position))
        {
            //Local damage
            healths[position]--;
            if (healths[position] <= 0)
            {
                // Remove slime from position
                game.ship.StationStatus[position] &= ~1;
                positions[position] = false;
            }

            //Full damage
            health--;
            if (health <= 0)
            {
                alive = false;
                beaten = true;
                OnClear();
            }
            return true;
        }
        return false;
    }

    public override bool AtPosition(int position)
    {
        return positions[position];
    }
}
#endregion
