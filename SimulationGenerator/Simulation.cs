using SpaceAlertSolver;
using System.Collections.Immutable;
using System.Text;

namespace SimulationGenerator;

public readonly record struct Simulation(string Trajectories, ImmutableArray<string> EventStrings, Act[][] Actions)
{
    public string Serialize()
    {
        StringBuilder builder = new();
        builder.AppendLine(Trajectories);
        foreach (string s in EventStrings)
        {
            builder.AppendLine(s);
        }
        foreach (Act[] acts in Actions)
        {
            builder.AppendLine(ActUtils.ActArrayToString(acts));
        }
        return builder.ToString();
    }
}
