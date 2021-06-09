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
        public ArmoredCatcher(Ship ship, Trajectory traj, int zone, int time) : base(ship,traj,zone,time)
        {
            health = 4;
            maxHealth = 4;
            shield = 3;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public ArmoredCatcher() { }

        public override ExThreat Clone(Ship ship)
        {
            ArmoredCatcher clone = new ArmoredCatcher();
            clone.CloneThreat(this, ship);
            clone.maxHealth = maxHealth;
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            health = Math.Min(health + 1, maxHealth);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 4);
        }
    }

    //ID: 1
    class Amoebe : ExThreat
    {
        int maxHealth;
        public Amoebe(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
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

        public override ExThreat Clone(Ship ship)
        {
            Amoebe clone = new Amoebe();
            clone.CloneThreat(this, ship);
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
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 4);
        }
    }

    //ID: 2
    class Battleship : ExThreat
    {
        public Battleship(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 2;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public Battleship() { }

        public override ExThreat Clone(Ship ship)
        {
            Battleship clone = new Battleship();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 3);
        }
    }

    //ID: 3
    class Hunter : ExThreat
    {
        public Hunter(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 4;
            shield = 2;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
        }

        public Hunter() { }

        public override ExThreat Clone(Ship ship)
        {
            Hunter clone = new Hunter();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 3);
        }
    }

    //ID: 4
    class GyroHunter : ExThreat
    {
        bool hasShield;
        public GyroHunter(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 4;
            shield = 1;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
            hasShield = true;
        }

        public GyroHunter() { }

        public override ExThreat Clone(Ship ship)
        {
            GyroHunter clone = new GyroHunter();
            clone.CloneThreat(this, ship);
            clone.hasShield = hasShield;
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
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
        public EnergyCloud(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 3;
            baseShield = 3;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public EnergyCloud() { }

        public override ExThreat Clone(Ship ship)
        {
            EnergyCloud clone = new EnergyCloud();
            clone.CloneThreat(this, ship);
            clone.baseShield = baseShield;
            return clone;
        }

        public override void ActX()
        {
            ship.shields[0] = 0;
            ship.shields[1] = 0;
            ship.shields[2] = 0;
        }
        public override void ActY()
        {
            for (int i = 0; i < 3; i++)
            {
                if (zone != i)
                    ship.game.BranchShieldFull(i);
            }

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
                    ship.game.BranchShieldFull(i);
            }

            for (int i = 0; i < 3; i++)
            {
                if (zone != i)
                    ship.DealDamage(i, 2);
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
        public Meteorite(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 0;
            speed = 5;
            scoreLose = 2;
            scoreWin = 4;
            rocketImmune = true;
        }

        public Meteorite() { }

        public override ExThreat Clone(Ship ship)
        {
            Meteorite clone = new Meteorite();
            clone.CloneThreat(this, ship);
            return clone;
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, health);
        }
    }

    //ID: 7
    class ImpulseBall : ExThreat
    {
        public ImpulseBall(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 1;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public ImpulseBall() { }

        public override ExThreat Clone(Ship ship)
        {
            ImpulseBall clone = new ImpulseBall();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
            ship.DealDamage(0, 1);
            ship.DealDamage(1, 1);
            ship.DealDamage(2, 1);
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
            ship.DealDamage(0, 1);
            ship.DealDamage(1, 1);
            ship.DealDamage(2, 1);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
            ship.DealDamage(0, 2);
            ship.DealDamage(1, 2);
            ship.DealDamage(2, 2);
        }
    }

    //ID: 8
    class SpaceCruiser : ExThreat
    {
        public SpaceCruiser(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 2;
            speed = 2;
            scoreLose = 2;
            scoreWin = 4;
        }

        public SpaceCruiser() { }

        public override ExThreat Clone(Ship ship)
        {
            SpaceCruiser clone = new SpaceCruiser();
            clone.CloneThreat(this, ship);
            return clone;
        }
        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            if (ship.shields[zone] == 0)
                ship.DealDamage(zone, 2);
            else
                ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);

            int dmg = 2;
            int bonus = 2 - ship.shields[zone];
            if (bonus > 0)
                dmg += bonus;

            ship.DealDamage(zone, dmg);
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
        public StealthHunter(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 4;
            shield = 2;
            speed = 3;
            scoreLose = 2;
            scoreWin = 4;
        }

        public StealthHunter() { }

        public override ExThreat Clone(Ship ship)
        {
            StealthHunter clone = new StealthHunter();
            clone.CloneThreat(this, ship);
            clone.visible = visible;
            return clone;
        }
        public override void ActX()
        {
            visible = true;
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
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
        public JellyFish(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
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

        public override ExThreat Clone(Ship ship)
        {
            JellyFish clone = new JellyFish();
            clone.CloneThreat(this, ship);
            clone.baseHealth = baseHealth;
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
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
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
            ship.DealDamage(0, 2);
            ship.DealDamage(1, 2);
            ship.DealDamage(2, 2);
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
        public SmallAsteroid(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 7;
            shield = 0;
            speed = 4;
            scoreLose = 3;
            scoreWin = 6;
            rocketImmune = true;
        }

        public SmallAsteroid() { }

        public override ExThreat Clone(Ship ship)
        {
            SmallAsteroid clone = new SmallAsteroid();
            clone.CloneThreat(this, ship);
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
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, health);
        }
        public override bool ProcessDamage()
        {
            if (health + shield - damage <= 0) // if we will die, branch
            {
                ship.game.BranchShieldFull(zone);
            }

            health = health - Math.Max(0, (damage - shield));
            damage = 0;

            if (health <= 0)
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
        public Kamikaze(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 2;
            speed = 4;
            scoreLose = 3;
            scoreWin = 6;
        }

        public Kamikaze() { }

        public override ExThreat Clone(Ship ship)
        {
            Kamikaze clone = new Kamikaze();
            clone.CloneThreat(this, ship);
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
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 6);
        }
    }

    //ID: 13
    class Swarm : ExThreat
    {
        public Swarm(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 3;
            shield = 0;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
            rocketImmune = true;
        }

        public Swarm() { }

        public override ExThreat Clone(Ship ship)
        {
            Swarm clone = new Swarm();
            clone.CloneThreat(this, ship);
            return clone;
        }

        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 1);
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
            for (int i = 0; i < 3; i++)
            {
                if (i == zone)
                    ship.DealDamage(i, 2);
                else
                    ship.DealDamage(i, 1);
            }
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
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
        public GhostHunter(Ship ship, Trajectory traj, int zone,int time) : base(ship, traj, zone, time)
        {
            health = 3;
            shield = 3;
            speed = 3;
            scoreLose = 3;
            scoreWin = 6;
        }

        public GhostHunter() { }

        public override ExThreat Clone(Ship ship)
        {
            GhostHunter clone = new GhostHunter();
            clone.CloneThreat(this, ship);
            clone.visible = visible;
            return clone;
        }
        public override void ActX()
        {
            visible = true;
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 3);
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

        public Scout(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 3;
            shield = 1;
            speed = 2;
            scoreLose = 3;
            scoreWin = 6;
        }

        public Scout() { }

        public override ExThreat Clone(Ship ship)
        {
            Scout clone = new Scout();
            clone.CloneThreat(this, ship);
            clone.act_y_i = act_y_i;
            return clone;
        }
        public override void ActX()
        {
            ship.scoutBonus = 1;
        }
        public override void ActY()
        {
            List<ExThreat> trts = ship.game.exThreats;

            while (act_y_i < trts.Count)
            {
                if (trts[act_y_i] is Scout || trts[act_y_i].beaten)
                    continue;
                trts[act_y_i].Move(1);
            }

            act_y_i = 0;
        }
        public override void ActZ()
        {
            ship.DealDamageIntern(zone, 3);
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
                ship.scoutBonus = 0;
            return r;
        }
    }
    #endregion
}
