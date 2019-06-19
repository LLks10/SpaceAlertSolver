﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpaceAlertSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create damage order
            Defects[] dfcts = new Defects[] {Defects.lift,Defects.reactor,Defects.shield,Defects.structure,Defects.weaponbot,Defects.weapontop,
                    Defects.lift,Defects.reactor,Defects.shield,Defects.structure,Defects.weaponbot,Defects.weapontop,
                    Defects.lift,Defects.reactor,Defects.shield,Defects.structure,Defects.weaponbot,Defects.weapontop};
            dfcts.Shuffle(0, 6);
            dfcts.Shuffle(6, 6);
            dfcts.Shuffle(12, 6);
            Console.WriteLine("Damage order (Red/White/Blue)");
            string dfctOrder = "";
            for(int i = 0; i < 18; i++)
            {
                if (i != 0 && i % 6 == 0)
                    dfctOrder += "\n";
                dfctOrder += dfcts[i].ToString()+" ";
            }
            Console.WriteLine(dfctOrder);
            Extension.doRandomDefect = false;
            Extension.defectOrder = dfcts;
            Console.WriteLine("-------");
            
            Random r = new Random();
            List<Event> events = new List<Event>();
            Player[] players = new Player[5];

            //Setup threat pool
            int comExThreatCount = 16;
            List<int> comExThreats = new List<int>();
            for (int i = 0; i < comExThreatCount; i++)
                comExThreats.Add(i);
            int sevExThreatCount = 13;
            List<int> sevExThreats = new List<int>();
            for (int i = 0; i < sevExThreatCount; i++)
                sevExThreats.Add(i + comExThreatCount);
            int comInThreatCount = 11;
            List<int> comInThreats = new List<int>();
            for (int i = 0; i < comInThreatCount; i++)
                comInThreats.Add(i);
            int sevInThreatCount = 11;
            List<int> sevInThreats = new List<int>();
            for (int i = 0; i < sevInThreatCount; i++)
                sevInThreats.Add(i + comInThreatCount);

            //Load trajectories
            Trajectory[] trajectories = new Trajectory[4];
            int[] ts = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            
            Console.WriteLine("'r' for random trajectories or xxxx to manually set");
            string resp = Console.ReadLine();
            if (resp == "r" || resp == "R")
            {
                for (int i = 0; i < 4; i++)
                {
                    int tsi = -1;
                    while (tsi == -1)
                        tsi = ts[r.Next(7)];
                    trajectories[i] = new Trajectory(tsi);
                    ts[tsi] = -1;
                }
            }
            else
            {
                for(int i = 0; i < 4; i++)
                    trajectories[i] = new Trajectory(int.Parse(resp[i].ToString())-1);
            }

            Console.WriteLine("Trajectories:");
            Console.WriteLine("Lt{0} Mt{1} Rt{2} It{3}", trajectories[0].number + 1, trajectories[1].number + 1, trajectories[2].number + 1, trajectories[3].number + 1);
            Console.WriteLine();
            Console.WriteLine("Add events: 'type 1..4 ec es ic is' 'turn 1...12' 'zone 0,1,2' | Type 'start' to start simulation");
            string[] zoneStr = new string[] { "Red", "White", "Blue", "Internal" };
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "start")
                    break;
                //Get threat
                else
                {
                    string[] strsplit = str.Split();
                    int type = int.Parse(strsplit[0]);
                    int t = int.Parse(strsplit[1]);
                    //External
                    if(type <= 2)
                    {
                        int z = int.Parse(strsplit[2]);
                        int thrt;
                        //Common
                        if(type == 1)
                        {
                            if(strsplit.Length > 3)
                            {
                                thrt = int.Parse(strsplit[3]);
                                comExThreats.Remove(thrt);
                            }
                            else
                            {
                                int thrtIdx = r.Next(comExThreats.Count);
                                thrt = comExThreats[thrtIdx];
                                comExThreats.RemoveAt(thrtIdx);
                            }
                        }
                        //Severe
                        else
                        {
                            if (strsplit.Length > 3)
                            {
                                thrt = int.Parse(strsplit[3]);
                                sevExThreats.Remove(thrt);
                            }
                            else
                            {
                                int thrtIdx = r.Next(sevExThreats.Count);
                                thrt = sevExThreats[thrtIdx];
                                sevExThreats.RemoveAt(thrtIdx);
                            }
                        }

                        Console.WriteLine("Loaded {0} on turn {1} in zone {2}", ThreatFactory.ExName(thrt), t, zoneStr[z]);
                        events.Add(new Event(true, t, z, thrt));
                    }
                    //Internal
                    else
                    {
                        int thrt;
                        if (type == 3)
                        {
                            if (strsplit.Length > 2)
                            {
                                thrt = int.Parse(strsplit[2]);
                                comInThreats.Remove(thrt);
                            }
                            else
                            {
                                int thrtIdx = r.Next(comInThreats.Count);
                                thrt = comInThreats[thrtIdx];
                                comInThreats.RemoveAt(thrtIdx);
                            }
                        }
                        else
                        {
                            if (strsplit.Length > 2)
                            {
                                thrt = int.Parse(strsplit[2]);
                                sevInThreats.Remove(thrt);
                            }
                            else
                            {
                                int thrtIdx = r.Next(sevInThreats.Count);
                                thrt = sevInThreats[thrtIdx];
                                sevInThreats.RemoveAt(thrtIdx);
                            }
                        }
                        Console.WriteLine("Loaded {0} on turn {1}", ThreatFactory.InName(thrt), t);
                        events.Add(new Event(false, t, 3, thrt));
                    }
                }
            }
            Event[] evArr = events.ToArray();

            //Random simulations
            Genetic genetic = new Genetic(500, 5, trajectories, evArr);
            while (true)
            {
                int sims = 0;
                while (sims < 10000)
                {
                    genetic.Cycle(trajectories, evArr);
                    sims++;
                }
                Console.WriteLine("Continue? (Y/N)  Reset: R");
                string ans = Console.ReadLine();
                if (ans == "N" || ans == "n")
                    break;
                if (ans == "R" || ans == "r")
                    genetic = new Genetic(500, 5, trajectories, evArr);
            }
            Console.ReadLine();
        }
    }



    public static class Extension
    {
        public static Random rng = new Random();
        public static bool doRandomDefect;
        public static Defects[] defectOrder;

        public static void Shuffle<T>(this IList<T> list, int bot, int range)
        {
            int n = range;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1)+bot;
                T value = list[k];
                list[k] = list[n+bot];
                list[n+bot] = value;
            }
        }
    }
}
