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
                if (P(_currentState.getScore(), newState.getScore(),
                        Temperature((double)iteration / maxIterations)) >= rng.NextDouble())
                {
                    _currentState = newState;
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
            return 1 - computationUsed; // value in [1, 2]
        }

        /**
         * <summary>The acceptance probability function</summary>
         * <param name="currentState">The score of the current state</param>
         * <param name="newState">The score of the candidate state</param>
         * <param name="temperature">The temperature</param>
         * <returns>A value in the range [0, 1] that represents the probability that this move should be taken</returns>
         */
        private double P(double currentState, double newState, double temperature)
        {
            if (newState > currentState) // newState is better
            {
                return 1.0;
            }
            else
            {
                return Math.Exp((newState - currentState) / temperature);
            }
        }
    }
}
