﻿using System.Collections.Immutable;

namespace SpaceAlertSolver;

public sealed class SimulatedAnnealing
{
    private List<int> _statsOfBest = null;

    private Gene _currentState;
    private double _highestScore = double.NegativeInfinity;
    private float _timing;
    private int _highestBlanks = int.MinValue;
    private Gene _bestState; // use for restarts

    public SimulatedAnnealing(int players, ImmutableArray<Trajectory> trajs, ImmutableArray<Event> evts)
    {
        _currentState = new Gene(players);
        _currentState.setEval(trajs, evts);
    }

    /**
     * <summary>Runs the simulated annealing algorithm</summary>
     * <param name="maxIterations">The number of iterations to do</param>
     * <param name="trajs">The trajectories</param>
     * <param name="evts">The events</param>
     * <param name="seed">[optional] The seed to use</param>
     */
    public void Run(int maxIterations, ImmutableArray<Trajectory> trajs, ImmutableArray<Event> evts, int seed, bool printDebug=Program.PRINT_DEBUG)
    {
        Random rng = new Random(seed);

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Game.Scores.Clear();

            Gene newState = _currentState.RandomNeighbour(rng, trajs, evts);
            if (P(_currentState, newState, Temperature((double)iteration / maxIterations)) >= rng.NextDouble())
            {
                _currentState = newState;
            }

            // print new best score
            if (_currentState.getScore() > _highestScore)
            {
                _statsOfBest = Game.Scores;
                Game.Scores = new List<int>();

                _highestScore = _currentState.getScore();
                _timing = iteration / (float)maxIterations;
                _highestBlanks = _currentState.getBlanks();
                _bestState = _currentState;

                if (printDebug)
                {
                    Console.WriteLine(_currentState.Rep() +
                        "Iteration: " + (iteration + 1) + 
                        " / " + maxIterations +
                        "\nScore: " + _currentState.getScore());
                }
            }
            else if (_currentState.getScore() == _highestScore && _currentState.getBlanks() > _highestBlanks)
            {
                _highestBlanks =  _currentState.getBlanks();
                _bestState = _currentState;
            }
        }
    }

    /**
     * <summary>Runs the simulated annealing algorithm with a random seed</summary>
     * <param name="maxIterations">The number of iterations to do</param>
     * <param name="trajs">The trajectories</param>
     * <param name="evts">The events</param>
     */
    public void Run(int maxIterations, ImmutableArray<Trajectory> trajs, ImmutableArray<Event> evts)
    {
        Run(maxIterations, trajs, evts, Guid.NewGuid().GetHashCode());
    }

    public Gene getCurrentGene()
    {
        return _currentState;
    }

    public Gene getBestGene()
    {
        return _bestState;
    }

    public List<int> getBestStats()
    {
        return _statsOfBest;
    }

    public double BestGeneAverage => _statsOfBest.Average();

    public double Timing => _timing;

    /**
     * <summary>The temperature function</summary>
     * <param name="computationUsed">The fraction of time used</param>
     * <remarks><c>computationUsed</c> must be in the range [0,1)</remarks>
     * <returns>The temperature</returns>
     */
    private double Temperature(double computationUsed)
    {
        double t = 1 - computationUsed; // (0, 1]

        //return (Math.Exp(t) - 1) / (Math.E - 1);
        return t;
    }

    /**
     * <summary>The acceptance probability function</summary>
     * <param name="currentState">The score of the current state</param>
     * <param name="newState">The score of the candidate state</param>
     * <param name="temperature">The temperature</param>
     * <returns>A value in the range [0, 1] that represents the probability that this move should be taken</returns>
     */
    private double P(Gene currentState, Gene newState, double temperature)
    {
        double c_score = currentState.getScore();
        double n_score = newState.getScore();
        int c_blanks = currentState.getBlanks();
        int n_blanks = newState.getBlanks();

        if (n_score > c_score) // newState is better
        {
            return 1.0;
        }
        /*else if (n_score == c_score)
        {
            if (n_blanks > c_blanks) // better in terms of blanks
            {
                //return 2 / (1 + Math.Exp(6 * temperature));
                return 0.5;
            }
            else if (n_blanks == c_blanks)
            {
                return 0.5;
            }
            else // worse in terms of blanks
            {
                return 2 / (1 + Math.Exp(-8 * temperature + 8));
            }
        }*/
        else // newState is worse
        {
            return Math.Exp(0.05 * (n_score - c_score) / temperature);
        }
    }
}
