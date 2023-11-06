using System.Diagnostics;
using System.Reflection;

namespace SpaceAlertSolver;

internal sealed class ThreatFactory
{
    public static ThreatFactory Instance { get; } = new ThreatFactory();

    private readonly List<Threat> _threatsById = new();
    private readonly List<string> _threatNamesById = new();
    private readonly List<(string, int)> _nameIdPairs = new();

    public IReadOnlyList<Threat> ThreatsById => _threatsById;
    public IReadOnlyList<string> ThreatNameById => _threatNamesById;

    private readonly List<int> _internalCommonThreatIds = new();
    private readonly List<int> _internalSevereThreatIds = new();
    private readonly List<int> _externalCommonThreatIds = new();
    private readonly List<int> _externalSevereThreatIds = new();

    public IReadOnlyList<int> InternalCommonThreatIds => _internalCommonThreatIds;
    public IReadOnlyList<int> InternalSevereThreatIds => _internalSevereThreatIds;
    public IReadOnlyList<int> ExternalCommonThreatIds => _externalCommonThreatIds;
    public IReadOnlyList<int> ExternalSevereThreatIds => _externalSevereThreatIds;

    private ThreatFactory()
    {
        MethodInfo[] threatCreators = typeof(Threat).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (MethodInfo method in threatCreators)
        {
            ThreatIdAttribute? idAttribute = method.GetCustomAttribute<ThreatIdAttribute>();
            if (idAttribute == null)
                continue;

            while (_threatNamesById.Count <= idAttribute.ThreatId);
            {
                _threatsById.Add(default);
                _threatNamesById.Add(string.Empty);
            }
            Debug.Assert(!_threatsById[idAttribute.ThreatId].IsInitialized, "Overwriting threat with same id");
            _threatsById[idAttribute.ThreatId] = (Threat)method.Invoke(null, null)!;
            
            string name;
            PrimaryThreatNameAttribute? primaryName = method.GetCustomAttribute<PrimaryThreatNameAttribute>();
            if (primaryName == null)
                name = $"Threat{idAttribute.ThreatId}";
            else
                name = primaryName.Name;
            _threatNamesById[idAttribute.ThreatId] = name;

            foreach (ThreatNameAttribute threatName in method.GetCustomAttributes<ThreatNameAttribute>())
            {
                _nameIdPairs.Add((threatName.Name, idAttribute.ThreatId));
            }

            if (idAttribute is InternalCommonThreatAttribute)
                _internalCommonThreatIds.Add(idAttribute.ThreatId);
            else if (idAttribute is InternalSevereThreatAttribute)
                _internalSevereThreatIds.Add(idAttribute.ThreatId);
            else if (idAttribute is ExternalCommonThreatAttribute)
                _externalCommonThreatIds.Add(idAttribute.ThreatId);
            else if (idAttribute is ExternalSevereThreatAttribute)
                _externalSevereThreatIds.Add(idAttribute.ThreatId);
            else
                throw new UnreachableException("Unknown threat type");
        }
    }

    public (string, int) FindThreatMatchingName(string threatName)
    {
        (string name, int id) bestPair = default;
        int shortestDistance = int.MaxValue;
        foreach (var (name, id) in _nameIdPairs)
        {
            int distance = LevenshteinDistance(name.ToLower(), threatName.ToLower());
            if (distance < shortestDistance)
            {
                bestPair = (name, id);
                shortestDistance = distance;
            }
        }
        return bestPair;
    }

    // https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560 Thanks!
    /// <summary>
    ///     Calculate the difference between 2 strings using the Levenshtein distance algorithm
    /// </summary>
    /// <param name="source1">First string</param>
    /// <param name="source2">Second string</param>
    /// <returns></returns>
    private static int LevenshteinDistance(string source1, string source2) //O(n*m)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;
        var matrix = new int[source1Length + 1, source2Length + 1];
        // First calculation, if one entry is empty return full length
        if (source1Length == 0)
            return source2Length;
        if (source2Length == 0)
            return source1Length;
        // Initialization of matrix with row size source1Length and columns size source2Length
        for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
        for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }
        // Calculate rows and collumns distances
        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }
        // return result
        return matrix[source1Length, source2Length];
    }
}
