﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    //Position
    //0  1  2
    //3  4  5   6-> outside
    public class Ship
    {
        public Player[] players;
        public Androids[] androids;
        public Game game;
        public int[] shields;
        public int[] shieldsCap;
        public int[] reactors;
        public int[] reactorsCap;
        public int[] laserDamage;
        public int[] plasmaDamage;
        public int pulseRange;
        public int[] damage;
        public bool[] liftUsed;
        public bool[] cannonFired;

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
            plasmaDamage = new int[] { 2, 1, 2 };
            cannonFired = new bool[6];
            liftUsed = new bool[3];
            pulseRange = 2;
            damage = new int[] { 0, 0, 0 };
            capsules = 3;
            rockets = 3;
            interceptorReady = true;
        }

        public void DealDamage(int zone, int amount)
        {
            shields[zone] -= amount + scoutBonus;

            //Excess damage
            if(shields[zone] < 0)
            {
                damage[zone] += -shields[zone];
                shields[zone] = 0;
            }
        }
    }
}
