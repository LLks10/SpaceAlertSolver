using System;
using System.Collections.Generic;

namespace SpaceAlertSolver
{
    public class Genetic
    {
        private List<Gene> genes;
        private static Random random = new Random();
        private static double mutationChance = 7/60;
        int generation = 0;
        private int players;

        public Genetic(int num, int players, Trajectory[] trajs, Event[] evs)
        {
            this.players = players;
            genes = new List<Gene>();
            for (int i = 0; i < num; i++)
            {
                genes.Add(new Gene(players));
            }

            for (int i = 0; i < num; i++)
                genes[i].setEval(trajs, evs);
        }

        public String Rep()
        {
            String output = "";

            for (int i = 0; i < genes.Count; i++)
            {
                output += genes[i].Rep() + "\n\n";
            }

            return output;
        }

        public void Cycle(Trajectory[] trajs, Event[] evs)
        {
            int cycleLength = genes.Count / 2;
            List<Gene> newGenes = new List<Gene>();
            generation++;
            // Tournaments
            for (int i = 0; i < cycleLength; i++)
            {
                // Select three genes
                Gene gene1 = this.genes[random.Next(0, genes.Count)];
                Gene gene2 = gene1;
                Gene gene3 = gene1;

                while (gene2 == gene1)
                {
                    gene2 = this.genes[random.Next(0, genes.Count)];
                }
                while (gene3 == gene1 || gene3 == gene2)
                {
                    gene3 = this.genes[random.Next(0, genes.Count)];
                }

                // Remove the worst gene
                Gene bestGene1, bestGene2;

                if (gene1.getScore() <= gene2.getScore() && gene1.getScore() <= gene3.getScore())
                {
                    this.genes.Remove(gene1);
                    bestGene1 = gene2;
                    bestGene2 = gene3;
                }
                else if (gene2.getScore() <= gene1.getScore() && gene2.getScore() <= gene3.getScore())
                {
                    this.genes.Remove(gene2);
                    bestGene1 = gene1;
                    bestGene2 = gene3;
                }
                else
                {
                    this.genes.Remove(gene3);
                    bestGene1 = gene1;
                    bestGene2 = gene2;
                }

                // Create a new gene
                int[] newGene = new int[players*12];

                for (int k = 0; k < players; k++)
                {
                    int split = random.Next(0, 12);

                    // Pre-split
                    for (int j = k * 12; j < split + k * 12; j++)
                    {
                        if (random.NextDouble() < mutationChance)
                        {
                            newGene[j] = random.Next(0, 8);
                        }
                        else
                        {
                            newGene[j] = bestGene1.getGene(j);
                        }
                    }

                    // Post-split
                    for (int j = split + k * 12; j < 12 + k * 12; j++)
                    {
                        if (random.NextDouble() < mutationChance)
                        {
                            newGene[j] = random.Next(0, 8);
                        }
                        else
                        {
                            newGene[j] = bestGene2.getGene(j);
                        }
                    }
                }

                newGenes.Add(new Gene(newGene));
            }

            // Post-tournaments
            foreach (Gene g in newGenes)
            {
                g.setEval(trajs, evs);
            }
            this.genes.AddRange(newGenes);

            int highest = int.MinValue;
            Gene best = null;
            foreach (Gene g in genes)
            {
                if (g.getScore() > highest)
                {
                    highest = g.getScore();
                    best = g;
                }
            }
            if(generation % 1000 == 0)
                Console.WriteLine("Gen {0} best: {1}", generation, highest);
            if (generation % 10000 == 0)
            {
                Console.WriteLine(best.Rep());
            }
        }
    }
}
