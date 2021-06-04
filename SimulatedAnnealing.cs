using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    class SimulatedAnnealing
    {
        private Gene _currentState;
        private int _bestScore = int.MinValue;
        private Gene _bestState; // use for restarts

        public SimulatedAnnealing(int players, Trajectory[] trajs, Event[] evts)
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
        public void Run(int maxIterations, Trajectory[] trajs, Event[] evts, int seed)
        {
            Random rng = new Random(seed);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                Gene newState = _currentState.RandomNeighbour(rng, trajs, evts);
                if (P(_currentState, newState,
                        Temperature((double)iteration / maxIterations)) >= rng.NextDouble())
                {
                    _currentState = newState;
                }

                // print new best score
                if (_currentState.getScore() > _bestScore)
                {
                    _bestScore = _currentState.getScore();
                    _bestState = _currentState;
                    Console.WriteLine(_currentState.Rep() +
                        "Iteration: " + (iteration + 1) + 
                        " / " + maxIterations +
                        "\nScore: " + _currentState.getScore());
                }
            }
        }

        /**
         * <summary>Runs the simulated annealing algorithm with a random seed</summary>
         * <param name="maxIterations">The number of iterations to do</param>
         * <param name="trajs">The trajectories</param>
         * <param name="evts">The events</param>
         */
        public void Run(int maxIterations, Trajectory[] trajs, Event[] evts)
        {
            Run(maxIterations, trajs, evts, Guid.NewGuid().GetHashCode());
        }

        public Gene getGene()
        {
            return _currentState;
        }

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
            int c_score = currentState.getScore();
            int n_score = newState.getScore();
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
                    return 2 / (1 + Math.Exp(12 * temperature));
                }
                else if (n_blanks == c_blanks)
                {
                    return 0.5;
                }
                else // worse in terms of blanks
                {
                    return 2 / (1 + Math.Exp(-8 * temperature - 8));
                }
            }*/
            else // newState is worse
            {
                return Math.Exp(0.05 * (n_score - c_score) / temperature);
            }
        }
    }
}
