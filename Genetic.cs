using System;
using System.Collections.Generic;

namespace SpaceAlertSolver
{
    public class Genetic
    {
        private List<Gene> genes;
        private static Random random = new Random();
        private static double mutationChance = 7.0 / 60.0;
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
                int ind1, ind2, ind3;
                ind1 = random.Next(0, genes.Count);
                ind2 = ind1;
                ind3 = ind1;

                while (ind2 == ind1)
                {
                    ind2 = random.Next(0, genes.Count);
                }
                while (ind3 == ind1 || ind3 == ind2)
                {
                    ind3 = random.Next(0, genes.Count);
                }

                Gene gene1 = genes[ind1];
                Gene gene2 = genes[ind2];
                Gene gene3 = genes[ind3];

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

                if ((gene1.getScore() < gene2.getScore() || gene1.getScore() == gene2.getScore() && gene1.getBlanks() <= gene2.getBlanks())
                    && gene1.getScore() < gene3.getScore() || gene1.getScore() == gene3.getScore() && gene1.getBlanks() <= gene3.getBlanks())
                { // 1 is worst
                    genes.RemoveAt(ind1);
                    bestGene1 = gene2;
                    bestGene2 = gene3;
                }
                else if ((gene2.getScore() < gene1.getScore() || gene2.getScore() == gene1.getScore() && gene2.getBlanks() <= gene1.getBlanks())
                    && gene2.getScore() < gene3.getScore() || gene2.getScore() == gene3.getScore() && gene2.getBlanks() <= gene3.getBlanks())
                { // 2 is worst
                    genes.RemoveAt(ind2);
                    bestGene1 = gene1;
                    bestGene2 = gene3;
                }
                else
                { // 3 is worst
                    genes.RemoveAt(ind3);
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

                newGenes.Add(new Gene(newGene, players));
            }

            // Post-tournaments
            foreach (Gene g in newGenes)
            {
                g.setEval(trajs, evs);
            }
            this.genes.AddRange(newGenes);

            if(generation % 1000 == 0)
            {
                int highest = int.MinValue;
                Gene best = null;
                foreach (Gene g in genes)
                {
                    if (g.getScore() > highest || g.getScore() == highest && g.getBlanks() > best.getBlanks())
                    {
                        highest = g.getScore();
                        best = g;
                    }
                }
                Console.WriteLine("Gen {0} best: {1} TB: {2}", generation, highest, best.getBlanks());
                if (generation % 10000 == 0)
                {
                    Console.WriteLine(best.Rep());
                        //best.setEval(trajs, evs);
                }
            }
               
        }
    }
}
