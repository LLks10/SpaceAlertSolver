//#define RUNLOCAL
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
        #if RUNLOCAL
        new AnnealingBenchmark() { Iterations = 200000 }.RunSimulatedAnnealing();
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
    public void RunSimulatedAnnealing()
    {
        SimulatedAnnealing sa = new(5, _trajectories, _events);
        sa.Run(Iterations, seed: Seed, printDebug: false);
    }
}