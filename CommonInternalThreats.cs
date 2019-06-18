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
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
            vulnerability = InDmgSource.android;
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
    #endregion
}
