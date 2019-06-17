using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    #region Normals
    //ID: 0
    class ArmoredCatcher : ExThreat
    {
        int maxHealth;
        public ArmoredCatcher(Ship ship, Trajectory traj, int zone) : base(ship,traj,zone)
        {
            health = 4;
            maxHealth = 4;
            shield = 3;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public override void ActX()
        {
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            health = Math.Min(health + 1, maxHealth);
        }
        public override void ActZ()
        {
            ship.DealDamage(zone, 4);
        }
    }

    //ID: 1
    class Amoebe : ExThreat
    {
        int maxHealth;
        public Amoebe(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 8;
            maxHealth = 8;
            shield = 0;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
            rocketImmune = true;
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
            ship.DealDamage(zone, 4);
        }
    }

    //ID: 2
    class Battleship : ExThreat
    {
        public Battleship(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 5;
            shield = 2;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public override void ActX()
        {
            ship.DealDamage(zone, 2);
        }
        public override void ActY()
        {
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.DealDamage(zone, 3);
        }
    }

    //ID: 3
    class Hunter : ExThreat
    {
        public Hunter(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 4;
            shield = 2;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
        }

        public override void ActX()
        {
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.DealDamage(zone, 3);
        }
    }

    //ID: 4
    class GyroHunter : ExThreat
    {
        bool hasShield;
        public GyroHunter(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 4;
            shield = 1;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
            hasShield = true;
        }

        public override void ActX()
        {
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.DealDamage(zone, 2);
        }
        public override bool ProcessDamage()
        {
            if(!hasShield)
                return base.ProcessDamage();
            else if(damage > shield)
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
        public EnergyCloud(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 5;
            shield = 3;
            baseShield = 3;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public override void ActX()
        {
            ship.shields[0] = 0;
            ship.shields[1] = 0;
            ship.shields[2] = 0;
        }
        public override void ActY()
        {
            for(int i = 0; i < 3; i++)
            {
                if (zone != i)
                    ship.DealDamage(i, 1);
            }
        }
        public override void ActZ()
        {
            for (int i = 0; i < 3; i++)
            {
                if (zone != i)
                    ship.DealDamage(i, 2);
            }
        }
        public override void DealDamage(int damage, int range, DmgSource source)
        {
            if (source == DmgSource.impulse && range >= distanceRange)
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
        public Meteorite(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 5;
            shield = 0;
            speed = 5;
            scoreLose = 2;
            scoreWin = 4;
            rocketImmune = true;
        }
        public override void ActZ()
        {
            ship.DealDamage(zone, health);
        }
    }

    //ID: 7
    class ImpulseBall : ExThreat
    {
        public ImpulseBall(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 5;
            shield = 1;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }
        public override void ActX()
        {
            ship.DealDamage(0, 1);
            ship.DealDamage(1, 1);
            ship.DealDamage(2, 1);
        }
        public override void ActY()
        {
            ship.DealDamage(0, 1);
            ship.DealDamage(1, 1);
            ship.DealDamage(2, 1);
        }
        public override void ActZ()
        {
            ship.DealDamage(0, 2);
            ship.DealDamage(1, 2);
            ship.DealDamage(2, 2);
        }
    }

    //ID: 8
    class SpaceCruiser : ExThreat
    {
        public SpaceCruiser(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 5;
            shield = 2;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }
        public override void ActX()
        {
            if (ship.shields[zone] == 0)
                ship.DealDamage(zone, 2);
            else
                ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            if (ship.shields[zone] <= 1)
                ship.DealDamage(zone, 4);
            else
                ship.DealDamage(zone, 2);
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
        public StealthHunter(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 4;
            shield = 2;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
        }
        public override void ActX()
        {
            visible = true;
        }
        public override void ActY()
        {
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ActY();
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
        public JellyFish(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 13;
            baseHealth = health;
            shield = -2;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
            rocketImmune = true;
        }
        public override void ActX()
        {
            ship.DealDamage(0, 1);
            ship.DealDamage(1, 1);
            ship.DealDamage(2, 1);
            int damage = baseHealth - health;
            health = Math.Min(baseHealth, health + damage / 2);
        }
        public override void ActY()
        {
            ActX();
        }
        public override void ActZ()
        {
            ship.DealDamage(0, 2);
            ship.DealDamage(1, 2);
            ship.DealDamage(2, 2);
        }
    }

    //ID: 11
    class SmallAsteroid : ExThreat
    {
        int revenge;
        public SmallAsteroid(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 7;
            shield = 0;
            speed = 4;
            scoreLose = 3;
            scoreWin = 6;
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
            ship.DealDamage(zone, health);
        }
        public override bool ProcessDamage()
        {
            health = health - (damage - shield);
            damage = 0;

            if (health < 0)
            {
                //Deal revenge damage
                ship.DealDamage(zone, revenge);
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
        public Kamikaze(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 5;
            shield = 2;
            speed = 4;
            scoreLose = 3;
            scoreWin = 6;
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
            ship.DealDamage(zone, 6);
        }
    }

    //ID: 13
    class Swarm : ExThreat
    {
        public Swarm(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 3;
            shield = 0;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
            rocketImmune = true;
        }
        public override void ActX()
        {
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            for(int i = 0; i < 3; i++)
            {
                if (i == zone)
                    ship.DealDamage(i, 2);
                else
                    ship.DealDamage(i, 1);
            }
        }
        public override void ActZ()
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == zone)
                    ship.DealDamage(i, 3);
                else
                    ship.DealDamage(i, 2);
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
        public GhostHunter(Ship ship, Trajectory traj, int zone) : base(ship, traj, zone)
        {
            health = 3;
            shield = 3;
            speed = 3;
            scoreLose = 3;
            scoreWin = 6;
        }
        public override void ActX()
        {
            visible = true;
        }
        public override void ActY()
        {
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.DealDamage(zone, 3);
        }
        public override void DealDamage(int damage, int range, DmgSource source)
        {
            if (!visible)
                return;
            if (source == DmgSource.rocket)
                return;

            base.DealDamage(damage, range, source);
        }
    }
    #endregion
}
