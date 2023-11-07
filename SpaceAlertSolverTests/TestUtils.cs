using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

public static class TestUtils
{
    internal static int GetThreatIdMatchingExactPrimaryName(string primaryName)
    {
        var (_, id) = ThreatFactory.Instance.FindThreatMatchingName(primaryName);
        if (ThreatFactory.Instance.ThreatNameById[id] != primaryName)
            throw new Exception("Primary name does match");
        return id;
    }

    internal static Threat GetThreatMatchingExactPrimaryName(string primaryName)
    {
        return ThreatFactory.Instance.ThreatsById[GetThreatIdMatchingExactPrimaryName(primaryName)];
    }

    public static ImmutableArray<Trajectory> GetTrajectoriesFromString(string str)
    {
        const int NUM_TRAJECTORIES = 4;

        Trajectory[] ret = new Trajectory[NUM_TRAJECTORIES];
        for (int i = 0; i < NUM_TRAJECTORIES; i++)
        {
            int n = str[i] - '1';
            ret[i] = new(n);
        }
        return ImmutableArray.Create(ret);
    }

    public static Player CreatePlayerFromActions(Act[] actions)
    {
        return new(ImmutableArray.Create(actions));
    }

    public static Player CreatePlayerFromActions(IEnumerable<Act> actions)
    {
        return new(actions.ToImmutableArray());
    }

    public static Player[] CreatePlayersFromActions(params Act[][] actions)
    {
        return actions.Select(CreatePlayerFromActions).ToArray();
    }

    public static Player[] CreatePlayersFromActions(IEnumerable<IEnumerable<Act>> actions)
    {
        return actions.Select(CreatePlayerFromActions).ToArray();
    }
}
