using System;

namespace SpaceAlertSolver
{
    public class Gene
    {
        private int[] gene;
        public int players, score, blanks, wins, losses;
        private static string[] playerColours = new string[] { "P", "R", "Y", "G", "B", "1", "2", "3", "4", "5" };
        public string debug;

        public Gene(int players)
        {
            this.players = players;

            Random random = new Random(Guid.NewGuid().GetHashCode());
            gene = new int[players*12];
            for (int i = 0; i < players*12; i++) // randomly fill gene
            {
                gene[i] = random.Next(0, 8);
            }
        }

        public Gene(int[] gene, int players)
        {
            this.gene = gene;
            this.players = players;
        }

        // copy constructor
        public Gene(Gene other)
        {
            this.gene = new int[other.gene.Length];
            other.gene.CopyTo(this.gene, 0);
            this.players = other.players;
        }

        /**
         * <summary>Returns a random neighbour of this gene/state</summary>
         * <remarks>The returned gene might be weighted to better suit simulated annealing</remarks>
         * <returns>A gene that is similar to this gene</returns>
         * <param name="trajs">The trajectories</param>
         * <param name="evts">The events</param>
         */
        public Gene RandomNeighbour(Random rng, Trajectory[] trajs, Event[] evts)
        {
            Gene neighbour = new Gene(this);
            int r_index = rng.Next(neighbour.gene.Length);
            int r_offset = rng.Next(1, 8);
            neighbour.gene[r_index] = (neighbour.gene[r_index] + r_offset) % 8;
            neighbour.setEval(trajs, evts);
            return neighbour;
        }

        public String Rep()
        {
            String output = "      1     2     3     4     5     6     7     8     9    10    11    12\n";
            for (int j = 0; j < players; j++)
            {
                output += playerColours[j] + " | ";
                for (int i = 0; i < 12; i++)
                {
                    switch (this.gene[i + j * 12])
                    {
                        case 0:
                            output += "blank ";
                            break;
                        case 1:
                            output += " blue ";
                            break;
                        case 2:
                            output += " red  ";
                            break;
                        case 3:
                            output += " lift ";
                            break;
                        case 4:
                            output += "  A   ";
                            break;
                        case 5:
                            output += "  B   ";
                            break;
                        case 6:
                            output += "  C   ";
                            break;
                        case 7:
                            output += "robot ";
                            break;
                    }
                }
                output += "\n";
            }
            
            return output;
        }

        public void setEval(Trajectory[] trajs, Event[] evts)
        {
            score = Evaluate(this.gene, trajs, evts);

            blanks = 0;
            for (int i = 0; i < players * 12; i++)
            {
                if (gene[i] == 0)
                {
                    blanks++;
                }
            }
        }

        private int Evaluate(int[] gene, Trajectory[] trajs, Event[] evts)
        {
            Player[] ps = new Player[players];
            for (int i = 0; i < players; i++)
                ps[i] = new Player();

            int idx = 0;
            for (int i = 0; i < players; i++)
            {
                ps[i].actions = new Act[12];
                for (int j = 0; j < 12; j++)
                {
                    switch (gene[idx])
                    {
                        case 0:
                            ps[i].actions[j] = Act.empty;
                            break;
                        case 1:
                            ps[i].actions[j] = Act.right;
                            break;
                        case 2:
                            ps[i].actions[j] = Act.left;
                            break;
                        case 3:
                            ps[i].actions[j] = Act.lift;
                            break;
                        case 4:
                            ps[i].actions[j] = Act.A;
                            break;
                        case 5:
                            ps[i].actions[j] = Act.B;
                            break;
                        case 6:
                            ps[i].actions[j] = Act.C;
                            break;
                        case 7:
                            ps[i].actions[j] = Act.fight;
                            break;
                    }
                    idx++;
                }
            }

            int scr;
            if (Extension.doRandomDefect)
            {
                scr = 0;
                //Run first game
                Game g = new Game();
                g.Setup(ps, trajs, evts);
                int s = g.Simulate();
                if (s > -40)
                    wins++;
                else
                    losses++;
                scr += s;

                if (g.isDeterministic)
                    scr *= 10;
                //Run multiple if non deterministic
                else
                {
                    for (int i = 0; i < 9; i++)
                    {
                        g = new Game();
                        g.Setup(ps, trajs, evts);
                        s = g.Simulate();
                        if (s > -40)
                            wins++;
                        else
                            losses++;
                        scr += s;
                    }
                }
            }
            else
            {
                Game g = new Game();
                g.Setup(ps, trajs, evts);
                scr = g.Simulate();
                debug = g.GetDebug();
            }

            return scr;
        }

        public int getScore()
        {
            return this.score;
        }

        public int getBlanks()
        {
            return blanks;
        }

        public int getGene(int pos)
        {
            return this.gene[pos];
        }


        //Runs a simulation and gives output
        public int RunSimulation(Trajectory[] trajs, Event[] evts)
        {
            Player[] ps = new Player[players];
            for (int i = 0; i < players; i++)
                ps[i] = new Player();

            int idx = 0;
            for (int i = 0; i < players; i++)
            {
                ps[i].actions = new Act[12];
                for (int j = 0; j < 12; j++)
                {
                    switch (gene[idx])
                    {
                        case 0:
                            ps[i].actions[j] = Act.empty;
                            break;
                        case 1:
                            ps[i].actions[j] = Act.right;
                            break;
                        case 2:
                            ps[i].actions[j] = Act.left;
                            break;
                        case 3:
                            ps[i].actions[j] = Act.lift;
                            break;
                        case 4:
                            ps[i].actions[j] = Act.A;
                            break;
                        case 5:
                            ps[i].actions[j] = Act.B;
                            break;
                        case 6:
                            ps[i].actions[j] = Act.C;
                            break;
                        case 7:
                            ps[i].actions[j] = Act.fight;
                            break;
                    }
                    idx++;
                }
            }
            Game g = new Game();
            g.Setup(ps, trajs, evts);
            return g.Simulate();
        }
    }
}