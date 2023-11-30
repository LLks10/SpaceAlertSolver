using SpaceAlertSolver;
using System.Collections.Immutable;

namespace SimulationGenerator;

public readonly record struct Simulation(ImmutableArray<Trajectory> Trajectories, ImmutableArray<string> EventStrings, Act[][] Actions)
{
}
