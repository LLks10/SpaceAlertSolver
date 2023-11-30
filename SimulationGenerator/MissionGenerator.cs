using SpaceAlertSolver;
using System.Collections.Immutable;
using System.Diagnostics;

namespace SimulationGenerator;

internal sealed class MissionGenerator
{
    private readonly Random _random = new();
    private static readonly double[] THREAT_COUNT_DISTRIBUTION = new double[] { 0, 3, 4, 6, 6, 8, 6, 3, 3 };

    private int GetRandomThreatCount()
    {
        return PickIndexWithDistribution(THREAT_COUNT_DISTRIBUTION);
    }

    private int PickIndexWithDistribution(double[] distribution)
    {
        double sum = distribution.Sum();
        double value = _random.NextDouble() * sum;
        double runningSum = 0;
        for (int i = 0; i < distribution.Length; i++)
        {
            runningSum += distribution[i];
            if (value < runningSum)
                return i;
        }
        throw new UnreachableException();
    }

    private ImmutableArray<Trajectory> PickRandomTrajectories()
    {
        int[] trajectories = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        int[] result = new int[4];
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = _random.Next(i, trajectories.Length);
            result[i] = trajectories[randomIndex];
            trajectories[randomIndex] = trajectories[i];
        }
        return result.Select(t => new Trajectory(t)).ToImmutableArray();
    }
}
