using SpaceAlertSolver;

namespace SpaceAlertSolverTests;

internal static class TestUtils
{
    public static Trajectory[] GetTrajectoriesFromString(string str)
    {
        const int NUM_TRAJECTORIES = 4;

        Trajectory[] ret = new Trajectory[NUM_TRAJECTORIES];
        for (int i = 0; i < NUM_TRAJECTORIES; i++)
        {
            int n = str[i] - '1';
            ret[i] = new(n);
        }
        return ret;
    }
}
