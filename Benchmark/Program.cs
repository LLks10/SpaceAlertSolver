﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SpaceAlertSolver;
using SpaceAlertSolverTests;
using System.Collections.Immutable;

namespace Benchmark;

internal sealed class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<AnnealingBenchmark>();
    }
}

[MemoryDiagnoser]
public class AnnealingBenchmark
{
    private readonly ImmutableArray<Trajectory> _trajectories = TestUtils.GetTrajectoriesFromString("1463");
    private readonly ImmutableArray<Event> _events = ImmutableArray.Create<Event>( new(true, 1, 1, 17), new(true, 3, 0, 12), new(false, 4, 3, 21), new(true, 6, 1, 15) );

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
        sa.Run(Iterations, _trajectories, _events, seed: Seed, printDebug: false);
    }
}