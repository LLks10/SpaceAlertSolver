using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SpaceAlertSolver
{
    class Program
    {
        public static int SEED = 17;

        static void Main(string[] args)
        {
            //Gene g = Run(SEED);

            int seed_start = 10;
            int seed_end = 20;

            double[] scores = new double[10];
            for (int seed = seed_start; seed < seed_end; seed++)
            {
                scores[seed-seed_start] = Run(seed).getScore();
            }
            for (int i = 0; i < scores.Length; i++)
            {
                Console.WriteLine($"{i + seed_start}: {scores[i]}");
            }

            Console.ReadLine();
        }

        static Gene Run(int seed)
        {
            //Extension.InitKeys(5, 8, 1 << 25);
            //Create damage order
            Random r = new Random(seed);
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
                for (int i = 0; i < 4; i++)
                    trajectories[i] = new Trajectory(int.Parse(resp[i].ToString()) - 1);
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
                    if (type <= 2)
                    {
                        int z = int.Parse(strsplit[2]);
                        int thrt;
                        //Common
                        if (type == 1)
                        {
                            if (strsplit.Length > 3)
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

                        Console.WriteLine("Loaded {0} on turn {1} in zone {2} ({3})", ThreatFactory.ExName(thrt), t, zoneStr[z], thrt);
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
                        Console.WriteLine("Loaded {0} on turn {1} ({2})", ThreatFactory.InName(thrt), t, thrt);
                        events.Add(new Event(false, t, 3, thrt));
                    }
                }
            }
            Event[] evArr = events.ToArray();

            //Random simulations
            /*Genetic genetic = new Genetic(400, 5, trajectories, evArr);
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
                    genetic = new Genetic(400, 5, trajectories, evArr);
            }
            Console.ReadLine();*/

            SimulatedAnnealing sa = new SimulatedAnnealing(players.Length, trajectories, evArr);
            sa.Run(2000000, trajectories, evArr, seed);
            Console.WriteLine(sa.getBestGene().Rep() + sa.getBestGene().getScore());
            Console.WriteLine("-----FINAL-----");
            //Console.WriteLine(sa.getBestGene().Rep());
            return sa.getBestGene();
        }
    }



    public static class Extension
    {
        public static ulong[][] keys;
        public static ulong[] hash;
        public static ulong hashSize;

        public static Random rng = new Random(Program.SEED);
        public static bool doRandomDefect;

        public static T[] CopyArray<T>(T[] a)
        {
            T[] ret = new T[a.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = a[i];
            }
            return ret;
        }

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

        public static void InitKeys(int pCount, int actCount, ulong hashSize)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            keys = new ulong[pCount*12][];
            for(int i = 0; i < keys.Length; i++)
            {
                keys[i] = new ulong[actCount];
                for(int j = 0; j < actCount; j++)
                {
                    byte[] bytes = new byte[8];
                    rng.GetBytes(bytes);
                    keys[i][j] = BitConverter.ToUInt64(bytes, 0);
                }
            }
            rng.Dispose();

            hash = new ulong[hashSize];
            Extension.hashSize = hashSize;
        }

        public static Player GetPlayer(string actions)
        {
            string[] splt = actions.Split();
            Player p = new Player();
            Act[] acts = new Act[12];
            int idx = 0;
            while(idx < 12)
            {
                switch (splt[idx])
                {
                    case "blank":
                        acts[idx] = Act.empty;
                        idx++;
                        break;
                    case "A":
                        acts[idx] = Act.A;
                        idx++;
                        break;
                    case "B":
                        acts[idx] = Act.B;
                        idx++;
                        break;
                    case "C":
                        acts[idx] = Act.C;
                        idx++;
                        break;
                    case "blue":
                        acts[idx] = Act.right;
                        idx++;
                        break;
                    case "red":
                        acts[idx] = Act.left;
                        idx++;
                        break;
                    case "lift":
                        acts[idx] = Act.lift;
                        idx++;
                        break;
                    case "robot":
                        acts[idx] = Act.fight;
                        idx++;
                        break;
                }
            }
            p.actions = acts;
            return p;
        }
    }
}
