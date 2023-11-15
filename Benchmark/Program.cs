//#define RUNANNEALING
//#define RUNSINGLE
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SpaceAlertSolver;
using SpaceAlertSolverTests;
using System.Collections.Immutable;

namespace Benchmark;

internal sealed class Program
{
    public static void Main()
    {
        #if RUNANNEALING
        new AnnealingBenchmark() { Iterations = 200000 }.SimulatedAnnealing();
        #elif RUNSINGLE
        new AnnealingBenchmark().SingleSimulation();
        #else
        BenchmarkRunner.Run<AnnealingBenchmark>();
        #endif
    }
}

[MemoryDiagnoser]
public class AnnealingBenchmark
{
    private readonly ImmutableArray<Trajectory> _trajectories = TestUtils.GetTrajectoriesFromString("1463");
    private readonly ImmutableArray<Event> _events = ImmutableArray.Create<Event>
    (
        "1 1 cryoshield frigate",
        "3 0 kamikaze",
        "4 power system overload",
        "6 1 scout"
    );
    private readonly Player[] _players = TestUtils.CreatePlayersFromActions
    (
        ActUtils.ParseActionsFromString(" aaaf   a dd"),
        ActUtils.ParseActionsFromString(" eaae dbbef "),
        ActUtils.ParseActionsFromString("  drc ce f d"),
        ActUtils.ParseActionsFromString("   db  f fda")
    );

    [Params(100000, 200000)]
    public int Iterations;

    //[Params(0, 1, 2, 3, 4)]
    public int Seed = 0;

    [IterationCleanup]
    public void IterationCleanup()
    {
        GamePool.Clear();
    }

    [Benchmark]
    public void SimulatedAnnealing()
    {
        SimulatedAnnealing sa = new(5, _trajectories, _events);
        sa.Run(Iterations, seed: Seed, printDebug: false);
    }

    [Benchmark]
    public void SingleSimulation()
    {
        List<SimulationStep> simulationStack = new();
        Game.InitSimulationStack(simulationStack, 4, _events);
        Game g = GamePool.GetGame();
        for (int i = 0; i < (int)1e5; i++)
        {
            g.Init(_players, _trajectories, simulationStack: simulationStack);
            g.Simulate();
        }
        GamePool.FreeGame(g);
    }
}