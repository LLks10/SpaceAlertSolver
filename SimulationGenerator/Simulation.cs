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
        builder.AppendLine("-");
        foreach (Act[] acts in Actions)
        {
            builder.AppendLine(ActUtils.ActArrayToString(acts));
        }
        return builder.ToString();
    }

    public static Simulation Deserialize(string str)
    {
        string[] lines = str.Split(Environment.NewLine);
        string trajectories = lines[0];
        List<string> eventStrings = new();
        int i = 1;
        while (lines[i] != "-")
        {
            eventStrings.Add(lines[i]);
            i++;
        }
        i++;
        List<Act[]> acts = new();
        while (i < lines.Length)
        {
            acts.Add(ActUtils.ParseActionsFromString(lines[i]));
            i++;
        }
        return new(trajectories, eventStrings.ToImmutableArray(), acts.ToArray());
    }
}
