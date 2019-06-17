using System;

namespace SpaceAlertSolver
{
    public class Gene
    {
        private int[] gene;
        private int score;

        public Gene()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            this.gene = new int[60];
            for (int i = 0; i < 60; i++) // randomly fill gene
            {
                gene[i] = random.Next(0, 8);
            }
        }

        public Gene(int[] gene)
        {
            this.gene = gene;
        }

        public String Rep()
        {
            String output = "  1     2     3     4     5     6     7     8     9    10    11    12\n";
            for (int j = 0; j < 5; j++)
            {
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
            this.score = Evaluate(this.gene, trajs, evts);
        }

        private int Evaluate(int[] gene, Trajectory[] trajs, Event[] evts)
        {
            Player[] ps = new Player[5];
            for (int i = 0; i < 5; i++)
                ps[i] = new Player();

            int idx = 0;
            for (int i = 0; i < 5; i++)
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

        public int getScore()
        {
            return this.score;
        }

        public int getGene(int pos)
        {
            return this.gene[pos];
        }
    }
}