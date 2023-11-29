//#define RUNANNEALING
//#define RUNSINGLE
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SpaceAlertSolver;
using SpaceAlertSolverTests;
using System.Collections.Immutable;
using System.Diagnostics;

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
    private readonly ImmutableArray<Event> _events = ImmutableArray.Create<Event>( new(true, 1, 1, 17), new(true, 3, 0, 12), new(false, 4, 3, 21), new(true, 6, 1, 15) );
    private readonly Player[] _players = TestUtils.CreatePlayersFromActions
    (
        ActUtils.ParseActionsFromString(" ardacca eaa"),
        ActUtils.ParseActionsFromString("dbc  cdbd df"),
        ActUtils.ParseActionsFromString("bcabafcreaeb"),
        ActUtils.ParseActionsFromString("bfeaaedfrbc ")
    );

    [Params(100000, 200000)]
    public int Iterations;

    //[Params(0, 1, 2, 3, 4)]
    public int Seed = 0;

    [Benchmark]
    public void SimulatedAnnealing()
    {
        SimulatedAnnealing sa = new(5, _trajectories, _events);
        sa.Run(Iterations, _trajectories, _events, seed: Seed, printDebug: false);
    }

    [Benchmark]
    public void SingleSimulation()
    {
        Game g = GamePool.GetGame();
        g.Init(_players, _trajectories, _events);
        Debug.Assert(16.5 == g.Simulate());
        GamePool.FreeGame(g);
    }
}