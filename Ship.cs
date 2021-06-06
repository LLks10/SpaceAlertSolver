using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    //Position
    //0  1  2
    //3  4  5   6-> outside

    //1  2  4
    //8 16 32   64 Bitmasks stations

    //1  2  4  Bitmasks zone
    public class Ship
    {
        public Player[] players;
        public Androids[] androids;
        public Game game;
        public Defects[] defectOrder;
        public int[] shields;
        public int[] shieldsCap;
        public int[] reactors;
        public int[] reactorsCap;
        public int[] laserDamage;
        public int[] plasmaDamage;
        public int[] BDefect, CDefect;
        public int[] stationStatus;
        public int pulseRange;
        public int[] damage;
        public bool[] fissured;

        //Bit flags
        public int liftUsed, liftReset;
        public int cannonFired;

        public int capsules;

        public int rockets;
        public bool rocketFired;
        public bool rocketReady;

        public bool interceptorReady;

        public int scoutBonus;

        public Ship(Player[] players, Game game)
        {
            this.players = players;
            this.game = game;
            //Setup
            shields = new int[] { 1, 1, 1 };
            shieldsCap = new int[] { 2, 3, 2 };
            reactors = new int[] { 2, 3, 2 };
            reactorsCap = new int[] { 3, 5, 3 };
            androids = new Androids[] { new Androids(2), new Androids(3) };
            laserDamage = new int[] { 4, 5, 4 };
            plasmaDamage = new int[] { 2, 2, 2 };
            stationStatus = new int[6];
            BDefect = new int[6];
            CDefect = new int[7];
            fissured = new bool[3];
            damage = new int[] { 0, 0, 0 };
            capsules = 3;
            rockets = 3;
            interceptorReady = true;

            if (Extension.doRandomDefect)
                Extension.ShuffleDefects();

            defectOrder = Extension.defectOrder;
        }

        public void DealDamage(int zone, int amount)
        {
            amount += scoutBonus;
            shields[zone] -= amount;
            int dmg = -shields[zone];

            if (fissured[zone])
                dmg *= 2;

            //Excess damage
            if (shields[zone] < 0)
            {
                ApplyDamage(zone, dmg);
                shields[zone] = 0;
            }
        }

        public void DealDamageIntern(int zone, int amount)
        {
            if (fissured[zone])
                amount *= 2;

            ApplyDamage(zone, amount);
        }

        void ApplyDamage(int zone, int amount)
        {
            if(amount > 0)
            {
                //Go over defects
                if(damage[zone] < 6)
                {
                    int idx = zone * 6 + damage[zone];
                    for (int i = 0; i < amount && i < 6-damage[zone]; i++)
                    {
                        if (idx >= defectOrder.Length)
                        {
                            Console.WriteLine(zone);
                            Console.WriteLine(damage[zone]);
                            Console.WriteLine(i);
                            Console.WriteLine(idx);
                        }
                        switch (defectOrder[idx])
                        {
                            case Defects.lift:
                                liftReset |= 1 << zone;
                                break;
                            case Defects.reactor:
                                reactorsCap[zone]--;
                                reactors[zone] = Math.Min(reactors[zone], reactorsCap[zone]);
                                break;
                            case Defects.shield:
                                shieldsCap[zone]--;
                                shields[zone] = Math.Min(shields[zone], shieldsCap[zone]);
                                break;
                            case Defects.weaponbot:
                                plasmaDamage[zone]--;
                                break;
                            case Defects.weapontop:
                                laserDamage[zone]--;
                                break;
                        }
                        idx++;
                    }
                }

                //Add damage
                damage[zone] += amount;
            }
        }
    }

    public enum Defects
    {
        structure,
        lift,
        weapontop,
        weaponbot,
        shield,
        reactor
    }
}
