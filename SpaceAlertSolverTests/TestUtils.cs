using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SpaceAlertSolverTests;

public static class TestUtils
{
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
