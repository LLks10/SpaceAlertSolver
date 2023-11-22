using System.Diagnostics;

namespace SpaceAlertSolver;

internal struct ThreatList
{
    public int Count { get; private set; } = 0;
    private Threat[] _threats = new Threat[8];
    private readonly List<int> _internalThreats = new();
    private readonly List<int> _externalThreats = new();
    private int _internalIndex, _externalIndex;

    public IEnumerable<int> InternalThreatIds => _internalThreats;
    public IEnumerable<int> ExternalThreatIds => _externalThreats;

    public int Current { get; private set; }

    public ThreatList() { }

    public void Clear()
    {
        Count = 0;
    }

    public ref Threat AddThreat(int threatId)
    {
        if (Count >= _threats.Length)
            ResizeArray(Count * 2);
        _threats[Count] = ThreatFactory.Instance.ThreatsById[threatId];
        ref Threat threat = ref _threats[Count];

        if (threat.IsExternal)
            _externalThreats.Add(Count);
        else
            _internalThreats.Add(Count);

        Count++;
        return ref threat;
    }

    private void ResizeArray(int newSize)
    {
        Debug.Assert(newSize > _threats.Length);
        Threat[] newArray = new Threat[newSize];
        _threats.CopyTo(newArray, 0);
        _threats = newArray;
    }

    public bool MoveNext()
    {
        if (_internalIndex >= _internalThreats.Count)
        {
            if (_externalIndex >= _externalThreats.Count)
                return false;

            Current = _externalThreats[_externalIndex];
            _externalIndex++;
            return true;
        }

        if (_externalIndex >= _externalThreats.Count)
        {
            Current = _internalThreats[_internalIndex];
            _internalIndex++;
            return true;
        }

        if (_internalThreats[_internalIndex] < _externalThreats[_externalIndex])
        {
            Current = _internalThreats[_internalIndex];
            _internalIndex++;
        }
        else
        {
            Current = _externalThreats[_externalIndex];
            _externalIndex++;
        }
        return true;
    }

    public void Reset()
    {
        _internalIndex = 0;
        _externalIndex = 0;
    }

    public ThreatList GetEnumerator()
    {
        Reset();
        return this;
    }
}
