using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    #region Normals
    //ID: 0
    class SaboteurRed : InThreat
    {
        public SaboteurRed(Ship ship, Trajectory traj, int time) : base(ship,traj,time)
        {
            health = 1;
            position = 4;
            speed = 4;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.android;
        }

        public SaboteurRed() { }

        public override InThreat Clone(Ship ship)
        {
            SaboteurRed clone = new SaboteurRed();
            clone.CloneThreat(this, ship);
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
            if (ship.reactors[z] == 0)
                ship.DealDamageIntern(z, 1);
            else
                ship.reactors[z]--;
        }
        public override void ActZ()
        {
            int z = position % 3;
            ship.DealDamageIntern(z, 2);
        }
    }
    //ID: 1
    class SaboteurBlue : InThreat
    {
        public SaboteurBlue(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 1;
            position = 4;
            speed = 4;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.android;
        }

        public SaboteurBlue() { }

        public override InThreat Clone(Ship ship)
        {
            SaboteurBlue clone = new SaboteurBlue();
            clone.CloneThreat(this, ship);
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
            if (ship.reactors[z] == 0)
                ship.DealDamageIntern(z, 1);
            else
                ship.reactors[z]--;
        }
        public override void ActZ()
        {
            int z = position % 3;
            ship.DealDamageIntern(z, 2);
        }
    }
    //ID: 2
    class SkirmisherRed : InThreat
    {
        public SkirmisherRed(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 1;
            position = 0;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public SkirmisherRed() { }

        public override InThreat Clone(Ship ship)
        {
            SkirmisherRed clone = new SkirmisherRed();
            clone.CloneThreat(this, ship);
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
            ship.DealDamageIntern(z, 3);
        }
    }
    //ID: 3
    class SkirmisherBlue : InThreat
    {
        public SkirmisherBlue(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 1;
            position = 2;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public SkirmisherBlue() { }

        public override InThreat Clone(Ship ship)
        {
            SkirmisherBlue clone = new SkirmisherBlue();
            clone.CloneThreat(this, ship);
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
            ship.DealDamageIntern(z, 3);
        }
    }
    //ID: 4
    class SoldiersRed : InThreat
    {
        public SoldiersRed(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 0;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public SoldiersRed() { }

        public override InThreat Clone(Ship ship)
        {
            SoldiersRed clone = new SoldiersRed();
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
            if (position != 2 && position != 5)
                position++;
        }
        public override void ActZ()
        {
            int z = position % 3;
            ship.DealDamageIntern(z, 4);
        }
    }
    //ID: 5
    class SoldiersBlue : InThreat
    {
        public SoldiersBlue(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 2;
            position = 5;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
            vulnerability = InDmgSource.android;
            fightBack = true;
        }

        public SoldiersBlue() { }

        public override InThreat Clone(Ship ship)
        {
            SoldiersBlue clone = new SoldiersBlue();
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
            if (position != 0 && position != 3)
                position--;
        }
        public override void ActZ()
        {
            int z = position % 3;
            ship.DealDamageIntern(z, 4);
        }
    }
    //ID: 6
    class Virus : InThreat
    {
        public Virus(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 3;
            position = 1;
            ship.CDefect[1]++;
            speed = 3;
            scoreLose = 3;
            scoreWin = 6;
            vulnerability = InDmgSource.C;
        }

        public Virus() { }

        public override InThreat Clone(Ship ship)
        {
            Virus clone = new Virus();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void OnClear()
        {
            ship.CDefect[1]--;
        }

        public override void ActX()
        {
            if (ship.reactors[0] > 0)
                ship.reactors[0]--;
            if (ship.reactors[1] > 0)
                ship.reactors[1]--;
            if (ship.reactors[2] > 0)
                ship.reactors[2]--;

        }
        public override void ActY()
        {
            for(int i = 0; i < ship.players.Length; i++)
            {
                if (ship.players[i].position < 6)
                    ship.players[i].Delay(ship.players[i].lastAction + 1);
            }
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(0, 1);
            ship.DealDamageIntern(1, 1);
            ship.DealDamageIntern(2, 1);
        }
    }
    //ID: 7
    class HackedShieldsRed : InThreat
    {
        public HackedShieldsRed(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 3;
            position = 0;
            ship.BDefect[0]++;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.B;
        }

        public HackedShieldsRed() { }

        public override InThreat Clone(Ship ship)
        {
            HackedShieldsRed clone = new HackedShieldsRed();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void OnClear()
        {
            ship.BDefect[0]--;
        }

        public override void ActX()
        {
            ship.shields[0] = 0;

        }
        public override void ActY()
        {
            ship.reactors[0] = 0;
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(0, 2);
        }
    }
    //ID: 8
    class HackedShieldsBlue : InThreat
    {
        public HackedShieldsBlue(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 3;
            position = 2;
            ship.BDefect[2]++;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.B;
        }

        public HackedShieldsBlue() { }

        public override InThreat Clone(Ship ship)
        {
            HackedShieldsBlue clone = new HackedShieldsBlue();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void OnClear()
        {
            ship.BDefect[2]--;
        }

        public override void ActX()
        {
            ship.shields[2] = 0;

        }
        public override void ActY()
        {
            ship.reactors[2] = 0;
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(2, 2);
        }
    }
    //ID: 9
    class OverheatedReactor : InThreat
    {
        public OverheatedReactor(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = 3;
            position = 4;
            ship.BDefect[4]++;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
            vulnerability = InDmgSource.B;
        }

        public OverheatedReactor() { }

        public override InThreat Clone(Ship ship)
        {
            OverheatedReactor clone = new OverheatedReactor();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void OnClear()
        {
            ship.BDefect[4]--;
        }

        public override void ActX()
        {
            ship.DealDamageIntern(1, ship.reactors[1]);

        }
        public override void ActY()
        {
            if (ship.capsules > 0)
                ship.capsules--;
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(1, 2);
        }
        public override bool DealDamage(int position, InDmgSource source)
        {
            bool r = base.DealDamage(position, source);

            if (r)
            {
                for(int i = 0; i < ship.players.Length; i++)
                {
                    if (ship.players[i].position == 3 || ship.players[i].position == 5)
                        ship.players[i].Kill();
                }
            }

            return r;
        }
    }
    //ID: 10
    class UnstableWarheads : InThreat
    {
        public UnstableWarheads(Ship ship, Trajectory traj, int time) : base(ship, traj, time)
        {
            health = ship.rockets;
            position = 5;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.C;
            if(health > 0)
                ship.CDefect[5]++;
            else
            {
                beaten = true;
                alive = false;
            }
        }

        public UnstableWarheads() { }

        public override InThreat Clone(Ship ship)
        {
            UnstableWarheads clone = new UnstableWarheads();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void OnClear()
        {
            ship.CDefect[5]--;
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(2, ship.rockets*3);
        }
    }
    #endregion
}
