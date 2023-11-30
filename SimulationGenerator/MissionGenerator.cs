using SpaceAlertSolver;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SimulationGenerator;

internal sealed class MissionGenerator
{
    private readonly Random _random = new();
    private static readonly double[] THREAT_COUNT_DISTRIBUTION = new double[] { 0, 3, 4, 6, 6, 8, 6, 3, 3 };
    private static readonly double[] PLAYER_COUNT_DISTRIBUTION = new double[] { 0, 0, 0, 1, 10, 10 };
    private const double INTERNAL_CHANCE = 0.3;

    public Simulation GetNextSimulation()
    {
        ImmutableArray<Trajectory> trajectories = PickRandomTrajectories();
        int numberOfThreats = GetRandomThreatCount();
        int numberOfPlayers = GetRandomPlayerCount();
        ImmutableArray<Event> events = GetRandomEvents(numberOfThreats);
        ImmutableArray<string> eventStrings = events
            .OrderBy(e => e.Turn)
            .Select(e => $"{e.Turn} {e.Zone} {(e.IsExternal ? ThreatFactory.ExName(e.CreatureId) : ThreatFactory.InName(e.CreatureId))}")
            .ToImmutableArray();

        Act[][] actions = GetPlayerActions(trajectories, events);
        return new(trajectories, eventStrings, actions);
    }

    private Act[][] GetPlayerActions(ImmutableArray<Trajectory> trajectories, ImmutableArray<Event> events)
    {
        throw new NotImplementedException();
    }

    private ImmutableArray<Event> GetRandomEvents(int count)
    {
        List<int> externalThreatPool = ThreatFactory.GetExternalThreatPool();
        List<int> internalThreatPool = ThreatFactory.GetInternalThreatPool();
        ThreatId[] threats = new ThreatId[count];
        for (int i = 0; i < count; i++)
        {
            if (_random.NextDouble() < INTERNAL_CHANCE)
            {
                int id = PickFromPool(internalThreatPool);
                threats[i] = new(false, id);
            }
            else
            {
                int id = PickFromPool(externalThreatPool);
                threats[i] = new(true, id);
            }
        }
        int[] turns = GetRandomTurns(count);

        Event[] events = new Event[count];
        for (int i = 0; i < count; i++)
        {
            int turn = turns[i];
            int threatId = threats[i].Id;
            int zone = GetRandomZone(threats[i].IsExternal);
            events[i] = new(threats[i].IsExternal, turn, zone, threatId);
        }
        
        return events.ToImmutableArray();
    }

    private int GetRandomZone(bool isExternal)
    {
        if (isExternal)
            return _random.Next(3);
        else
            return 3;
    }

    private int GetRandomThreatCount()
    {
        return PickIndexWithDistribution(THREAT_COUNT_DISTRIBUTION);
    }

    private int GetRandomPlayerCount()
    {
        return PickIndexWithDistribution(PLAYER_COUNT_DISTRIBUTION);
    }

    private int[] GetRandomTurns(int count)
    {
        int[] turns = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        int[] ret = new int[count];
        for (int i = 0; i < count; i++)
        {
            int r = _random.Next(i, turns.Length);
            ret[i] = turns[r];
            turns[r] = turns[i];
        }
        return ret;
    }

    private int PickIndexWithDistribution(double[] distribution)
    {
        double sum = distribution.Sum();
        double value = _random.NextDouble() * sum;
        double runningSum = 0;
        for (int i = 0; i < distribution.Length; i++)
        {
            runningSum += distribution[i];
            if (value < runningSum)
                return i;
        }
        throw new UnreachableException();
    }

    private int PickFromPool(List<int> pool)
    {
        int r = _random.Next(pool.Count);
        int t = pool[r];
        pool[r] = pool[^1];
        pool.RemoveAt(pool.Count - 1);
        return t;
    }

    private ImmutableArray<Trajectory> PickRandomTrajectories()
    {
        int[] trajectories = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        int[] result = new int[4];
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = _random.Next(i, trajectories.Length);
            result[i] = trajectories[randomIndex];
            trajectories[randomIndex] = trajectories[i];
        }
        return result.Select(t => new Trajectory(t)).ToImmutableArray();
    }

    private readonly record struct ThreatId(bool IsExternal, int Id)
    {
    }
}
