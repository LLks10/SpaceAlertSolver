using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    #region Common
    //ID: 16
    class Fregat : ExThreat
    {
        public Fregat(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 7;
            shield = 2;
            speed = 2;
            scoreLose = 4;
            scoreWin = 8;
        }

        public Fregat() { }

        public override ExThreat Clone(Ship ship)
        {
            Fregat clone = new Fregat();
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
            ship.DealDamage(zone, 3);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 4);
        }
    }
    //ID: 17
    class GyroFregat : ExThreat
    {
        bool hasShield;
        public GyroFregat(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 7;
            shield = 1;
            speed = 2;
            scoreLose = 4;
            scoreWin = 8;
            hasShield = true;
        }

        public GyroFregat() { }

        public override ExThreat Clone(Ship ship)
        {
            GyroFregat clone = new GyroFregat();
            clone.CloneThreat(this, ship);
            clone.hasShield = hasShield;
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
            ship.DealDamage(zone, 3);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 4);
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
        public WarDeck(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 9;
            shield = 2;
            speed = 1;
            scoreLose = 4;
            scoreWin = 8;
        }

        public WarDeck() { }

        public override ExThreat Clone(Ship ship)
        {
            WarDeck clone = new WarDeck();
            clone.CloneThreat(this, ship);
            return clone;
        }
        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
            speed++;
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 3);
            shield++;
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 5);
        }
    }
    //ID: 19
    class InterStellarOctopus : ExThreat
    {
        int maxHealth;
        public InterStellarOctopus(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
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

        public override ExThreat Clone(Ship ship)
        {
            InterStellarOctopus clone = new InterStellarOctopus();
            clone.CloneThreat(this, ship);
            clone.maxHealth = maxHealth;
            return clone;
        }
        public override void ActX()
        {
            if (health < maxHealth)
            {
                ship.game.BranchShieldFull(0);
                ship.game.BranchShieldFull(1);
                ship.game.BranchShieldFull(2);
            }

            if(health < maxHealth)
            {
                ship.DealDamage(0, 1);
                ship.DealDamage(1, 1);
                ship.DealDamage(2, 1);
            }
        }
        public override void ActY()
        {
            if (health < maxHealth)
            {
                ship.game.BranchShieldFull(0);
                ship.game.BranchShieldFull(1);
                ship.game.BranchShieldFull(2);
            }

            if (health < maxHealth)
            {
                ship.DealDamage(0, 2);
                ship.DealDamage(1, 2);
                ship.DealDamage(2, 2);
            }
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2*health);
        }
    }
    //ID: 20
    class Maelstorm : ExThreat
    {
        int baseShield;
        public Maelstorm(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 8;
            shield = 3;
            baseShield = 3;
            speed = 2;
            scoreLose = 4;
            scoreWin = 8;
        }

        public Maelstorm() { }

        public override ExThreat Clone(Ship ship)
        {
            Maelstorm clone = new Maelstorm();
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

            for (int i = 0; i < 3; i++)
            {
                if (zone != i)
                    ship.DealDamage(i, 2);
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
                    ship.DealDamage(i, 3);
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
        public Asteroid(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 9;
            shield = 0;
            speed = 3;
            scoreLose = 4;
            scoreWin = 8;
            rocketImmune = true;
        }

        public Asteroid() { }

        public override ExThreat Clone(Ship ship)
        {
            Asteroid clone = new Asteroid();
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
            revenge++;
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, health);
        }
        public override bool ProcessDamage()
        {
            if (health + shield - damage <= 0) // branch if we would die
            {
                ship.game.BranchShieldFull(zone);
            }

            bool v = base.ProcessDamage();
            //Revenge damage if destroyed
            if (v)
                ship.DealDamage(zone, revenge*2);
            return v;
        }
    }
    //ID: 22
    class ImpulseSatellite : ExThreat
    {
        public ImpulseSatellite(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 4;
            shield = 2;
            speed = 3;
            scoreLose = 4;
            scoreWin = 8;
        }

        public ImpulseSatellite() { }

        public override ExThreat Clone(Ship ship)
        {
            ImpulseSatellite clone = new ImpulseSatellite();
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
            ship.DealDamage(0, 2);
            ship.DealDamage(1, 2);
            ship.DealDamage(2, 2);
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(1);
            ship.game.BranchShieldFull(2);
            ship.DealDamage(0, 3);
            ship.DealDamage(1, 3);
            ship.DealDamage(2, 3);
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
        public Nemesis(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 9;
            shield = 1;
            speed = 3;
            scoreLose = 0;
            scoreWin = 12;
        }

        public Nemesis() { }

        public override ExThreat Clone(Ship ship)
        {
            Nemesis clone = new Nemesis();
            clone.CloneThreat(this, ship);
            return clone;
        }
        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 1);
            health--;
            if (health <= 0)
            {
                beaten = true;
                alive = false;
            }
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
            health -= 2;
            if (health <= 0)
            {
                beaten = true;
                alive = false;
            }
        }
        public override void ActZ()
        {
            ship.damage[0] = 7;
            ship.damage[1] = 7;
            ship.damage[2] = 7;
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
                ship.game.BranchShieldFull(0);
                ship.game.BranchShieldFull(1);
                ship.game.BranchShieldFull(2);

                ship.DealDamage(0, 1);
                ship.DealDamage(1, 1);
                ship.DealDamage(2, 1);
            }
            return base.ProcessDamage();
        }
    }
    //ID: 24
    class NebulaCrab : ExThreat
    {
        public NebulaCrab(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 7;
            shield = 2;
            speed = 2;
            scoreLose = 6;
            scoreWin = 12;
            rocketImmune = true;
        }

        public NebulaCrab() { }

        public override ExThreat Clone(Ship ship)
        {
            NebulaCrab clone = new NebulaCrab();
            clone.CloneThreat(this, ship);
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
            ship.game.BranchShieldFull(0);
            ship.game.BranchShieldFull(2);
            ship.DealDamage(0, 5);
            ship.DealDamage(2, 5);
        }
    }
    //ID: 25
    class PsionicSatellite : ExThreat
    {
        public PsionicSatellite(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 5;
            shield = 2;
            speed = 2;
            scoreLose = 6;
            scoreWin = 12;
        }

        public PsionicSatellite() { }

        public override ExThreat Clone(Ship ship)
        {
            PsionicSatellite clone = new PsionicSatellite();
            clone.CloneThreat(this, ship);
            return clone;
        }
        public override void ActX()
        {
            for(int i = 0; i < ship.players.Length; i++)
            {
                Player p = ship.players[i];
                if (p.position % 3 == zone && p.position < 6)
                    p.Delay(p.lastAction + 1);
            }
        }
        public override void ActY()
        {
            for (int i = 0; i < ship.players.Length; i++)
            { 
                Player p = ship.players[i];
                if(p.position < 6)
                    p.Delay(p.lastAction + 1);
            }
        }
        public override void ActZ()
        {
            for (int i = 0; i < ship.players.Length; i++)
            {
                Player p = ship.players[i];
                if (p.position < 6)
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
        public LargeAsteroid(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 11;
            shield = 0;
            speed = 2;
            scoreLose = 6;
            scoreWin = 12;
            rocketImmune = true;
        }

        public LargeAsteroid() { }

        public override ExThreat Clone(Ship ship)
        {
            LargeAsteroid clone = new LargeAsteroid();
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
            revenge++;
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, health);
        }
        public override bool ProcessDamage()
        {
            if (health + shield - damage <= 0)
            {
                ship.game.BranchShieldFull(zone);
            }

            bool v = base.ProcessDamage();
            //Revenge damage if destroyed
            if (v)
                ship.DealDamage(zone, revenge * 3);
            return v;
        }
    }

    //ID: 27
    class Moloch : ExThreat
    {
        bool increaseShield = false;
        public Moloch(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 10;
            shield = 3;
            speed = 1;
            scoreLose = 6;
            scoreWin = 12;
        }

        public Moloch() { }

        public override ExThreat Clone(Ship ship)
        {
            Moloch clone = new Moloch();
            clone.CloneThreat(this, ship);
            clone.increaseShield = increaseShield;
            return clone;
        }
        public override void ActX()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 2);
            speed += 2;
        }
        public override void ActY()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 3);
            speed += 2;
        }
        public override void ActZ()
        {
            ship.game.BranchShieldFull(zone);
            ship.DealDamage(zone, 7);
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
        public Behemoth(Ship ship, Trajectory traj, int zone, int time) : base(ship, traj, zone, time)
        {
            health = 7;
            maxHealth = health;
            shield = 4;
            speed = 2;
            scoreLose = 6;
            scoreWin = 12;
        }

        public Behemoth() { }

        public override ExThreat Clone(Ship ship)
        {
            Behemoth clone = new Behemoth();
            clone.CloneThreat(this, ship);
            clone.maxHealth = maxHealth;
            return clone;
        }
        public override void ActX()
        {
            if (maxHealth - health < 2)
            {
                ship.game.BranchShieldFull(zone);
                ship.DealDamage(zone, 2);
            }
        }
        public override void ActY()
        {
            if (maxHealth - health < 3)
            {
                ship.game.BranchShieldFull(zone);
                ship.DealDamage(zone, 3);
            }
        }
        public override void ActZ()
        {
            if (maxHealth - health < 6)
            {
                ship.game.BranchShieldFull(zone);
                ship.DealDamage(zone, 6);
            }
        }
        public override void DealDamage(int damage, int range, ExDmgSource source)
        {
            //Valid cause interceptor performs range check
            if (source == ExDmgSource.intercept && damage == 3)
            {
                damage = 9;
                //Kill player
                Player[] ps = ship.players;
                for(int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].inIntercept)
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
}
