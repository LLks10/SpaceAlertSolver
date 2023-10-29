using System.Diagnostics;

namespace SpaceAlertSolver;

/// <summary>
/// Change <see cref="user_actions"/> to set the actions of human players.
/// Human players move before ai players.
/// </summary>
public class Gene
{
    private static Act[][] user_actions = new Act[][] {
        //ParseActions("craadcc ceb "),
        //ParseActions("rc fcefdbaac"),
    };

    private int[] gene;
    public int players, blanks;
    double score;
    private static string[] playerColours = new string[10] { "P", "R", "Y", "G", "B", "1", "2", "3", "4", "5" };
    public string debug;

    private static Func<Gene, Random, Gene>[] operators
        = new Func<Gene, Random, Gene>[]        { PointMutation, ForwardShift, BackwardShift, Delay, SwapPlayers };
    private static int[] op_chances = new int[] { 100,           115,          130,           145,   146 };

    public Gene(int players)
    {
        Debug.Assert(players >= 0 && players < 10);

        this.players = players;

        Random random = new Random(Program.SEED);
        gene = new int[players*12];
        for (int i = 0; i < players*12; i++) // randomly fill gene
        {
            gene[i] = random.Next(0, 8);
        }
    }

    public Gene(int[] gene, int players)
    {
        Debug.Assert(players >= 0 && players < 10
            && gene.Length == players * 12);

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
     * <summary>Parses a string to an action array</summary>
     * <returns>Act array of length 12</returns>
     * <param name="actions">The action string to parse, should have length 12</param>
     */
    private static Act[] ParseActions(string actions)
    {
        if (actions.Length != 12)
        {
            throw new ArgumentException("The action string must be 12 characters long");
        }

        Act[] ret = new Act[12];
        for (int i = 0; i < 12; i++)
        {
            switch (actions[i])
            {
                case 'a':
                    ret[i] = Act.A;
                    break;
                case 'b':
                    ret[i] = Act.B;
                    break;
                case 'c':
                    ret[i] = Act.C;
                    break;
                case 'd':
                    ret[i] = Act.Lift;
                    break;
                case 'f':
                    ret[i] = Act.Fight;
                    break;
                case 'r':
                    ret[i] = Act.Right;
                    break;
                case 'e':
                    ret[i] = Act.Left;
                    break;
                case ' ':
                    ret[i] = Act.Empty;
                    break;
                default:
                    throw new FormatException($"Invalid character <{actions[i]}>");
            }
        }

        return ret;
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
        Debug.Assert(op_chances.Length == operators.Length);

        int r = rng.Next(op_chances[op_chances.Length-1]);
        int bs = Array.BinarySearch(op_chances, r);
        int op_i;

        if (bs < 0)
        {
            op_i = ~bs;
        }
        else
        {
            op_i = bs + 1;
        }

        Gene ret = operators[op_i].Invoke(this, rng);
        ret.setEval(trajs, evts);
        return ret;
    }

    // START OPERATORS

    private static Gene PointMutation(Gene g, Random rng)
    {
        Gene neighbour = new Gene(g);

        int r_index = rng.Next(neighbour.gene.Length);
        int r_offset = rng.Next(1, 8);
        neighbour.gene[r_index] = (neighbour.gene[r_index] + r_offset) % 8;
        
        return neighbour;
    }

    private static Gene ForwardShift(Gene g, Random rng)
    {
        Gene neighbour = new Gene(g);

        int r_index = rng.Next(neighbour.gene.Length); // random action
        int end_index = (r_index / 12 + 1) * 12; // past the last action of the player

        int action_to_move = neighbour.gene[r_index];
        neighbour.gene[r_index] = 0;

        for (int i = r_index + 1; i < end_index && action_to_move != 0; i++)
        {
            int t = neighbour.gene[i];
            neighbour.gene[i] = action_to_move;
            action_to_move = t;
        }

        return neighbour;
    }

    private static Gene BackwardShift(Gene g, Random rng)
    {
        Gene neighbour = new Gene(g);

        int r_index = rng.Next(neighbour.gene.Length); // random action
        int end_index = r_index / 12 * 12; // first action of the player

        int action_to_move = neighbour.gene[r_index];
        neighbour.gene[r_index] = 0;

        for (int i = r_index - 1; i >= end_index && action_to_move != 0; i--)
        {
            int t = neighbour.gene[i];
            neighbour.gene[i] = action_to_move;
            action_to_move = t;
        }

        return neighbour;
    }

    private static Gene SwapPlayers(Gene g, Random rng)
    {
        Gene neighbour = new Gene(g);

        int player1 = rng.Next(neighbour.players);
        int player2 = (player1 + rng.Next(1, neighbour.players)) % neighbour.players;

        int p1 = player1 * 12;
        int p2 = player2 * 12;

        for (int i = 0; i < 12; i++)
        { // swap all 12 actions
            int t = neighbour.gene[p1];
            neighbour.gene[p1] = neighbour.gene[p2];
            neighbour.gene[p2] = t;

            p1++;
            p2++;
        }

        return neighbour;
    }

    private static Gene Delay(Gene g, Random rng)
    {
        Gene neighbour = new Gene(g);

        int r_index = rng.Next(neighbour.gene.Length); // random action
        int end_index = (r_index / 12 + 1) * 12; // past the last action of the player

        neighbour.gene[r_index] = 0; // blank at r_index

        if (end_index - r_index > 1) // if not instantly at end
        {
            int action_to_move = neighbour.gene[r_index + 1];
            neighbour.gene[r_index + 1] = 0;
            for (int i = r_index + 2; i < end_index && action_to_move != 0; i++)
            {
                int t = neighbour.gene[i];
                neighbour.gene[i] = action_to_move;
                action_to_move = t;
            }
        }

        return neighbour;
    }

    // END OPERATORS

    public String Rep()
    {
        String output = "      1     2     3     4     5     6     7     8     9    10    11    12\n";
        for (int j = 0; j < players; j++)
        {
            output += playerColours[j] + " | ";
            for (int i = 0; i < 12; i++)
            {
                Act a = (Act)gene[i + j * 12];
                output += a.ToStr().Pad(6);
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

    private double Evaluate(int[] gene, Trajectory[] trajs, Event[] evts)
    {
        Player[] ps = new Player[user_actions.Length + players];
        for (int i = user_actions.Length; i < players + user_actions.Length; i++)
            ps[i] = new Player();

        // initialize user players
        for (int i = 0; i < user_actions.Length; i++)
        {
            ps[i] = new Player(user_actions[i]);
        }

        // initialize random players
        int idx = 0;
        for (int i = user_actions.Length; i < players + user_actions.Length; i++)
        {
            ps[i].actions = new Act[12];
            for (int j = 0; j < 12; j++)
            {
                switch (gene[idx])
                {
                    case 0:
                        ps[i].actions[j] = Act.Empty;
                        break;
                    case 1:
                        ps[i].actions[j] = Act.Right;
                        break;
                    case 2:
                        ps[i].actions[j] = Act.Left;
                        break;
                    case 3:
                        ps[i].actions[j] = Act.Lift;
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
                        ps[i].actions[j] = Act.Fight;
                        break;
                }
                idx++;
            }
        }

        Game g = new Game(ps, trajs, evts);
        return g.Simulate();
    }

    public double getScore()
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
    /*public int RunSimulation(Trajectory[] trajs, Event[] evts)
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
    }*/
}