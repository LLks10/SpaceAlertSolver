using System.Diagnostics;

namespace SpaceAlertSolver;

public struct ThreatList
{
    private int _count = 0;
    private Threat[] _threats = new Threat[8];
    private readonly List<int> _internalThreats = new();
    private readonly List<int> _externalThreats = new();

    public IEnumerable<int> InternalThreatIndices => _internalThreats;
    public IEnumerable<int> ExternalThreatIndices => _externalThreats;

    public ThreatList() { }

    public ref Threat this[int i]
    {
        get
        {
            Debug.Assert(!_threats[i].Beaten);
            return ref _threats[i];
        }
    }

    public void Clear()
    {
        _count = 0;
    }

    public ref Threat AddThreat(int threatId)
    {
        if (_count >= _threats.Length)
            ResizeArray(_count * 2);
        _threats[_count] = ThreatFactory.Instance.ThreatsById[threatId];
        ref Threat threat = ref _threats[_count];

        if (threat.IsExternal)
            _externalThreats.Add(_count);
        else
            _internalThreats.Add(_count);

        _count++;
        return ref threat;
    }

    public void CopyTo(ThreatList other)
    {
        if (other._threats.Length < _count)
        {
            int newSize = other._threats.Length * 2;
            while (newSize < _count)
                newSize *= 2;
            other.ResizeArray(newSize);
        }

        Array.Copy(_threats, 0, other._threats, 0, _count);
        other._count = _count;
    }
    
    public int CleanBeatenThreats()
    {
        int score = 0;
        for (int i = _internalThreats.Count - 1; i >= 0; i--)
        {
            int index = _internalThreats[i];
            if (_threats[index].Beaten)
            {
                if (_threats[index].Alive)
                    score += _threats[index].ScoreLose;
                else
                    score += _threats[index].ScoreWin;
                _threats[index].OnBeaten();
                _internalThreats.RemoveAt(i);
            }
        }
        for (int i = _externalThreats.Count - 1; i >= 0; i--)
        {
            int index = _externalThreats[i];
            if (_threats[index].Beaten)
            {
                if (_threats[index].Alive)
                    score += _threats[index].ScoreLose;
                else
                    score += _threats[index].ScoreWin;
                _threats[index].OnBeaten();
                _externalThreats.RemoveAt(i);
            }
        }
        return score;
    }

    private void ResizeArray(int newSize)
    {
        Debug.Assert(newSize > _threats.Length);
        Threat[] newArray = new Threat[newSize];
        _threats.CopyTo(newArray, 0);
        _threats = newArray;
    }

    public Enumerator GetEnumerator()
    {
        return new(this);
    }

    public ReverseEnumerator GetReverseEnumerator()
    {
        return new(this);
    }

    public struct Enumerator
    {
        private readonly List<int> _internalIndices;
        private readonly List<int> _externalIndices;
        private int _internalIndex;
        private int _externalIndex;

        public int Current { get; private set; }

        internal Enumerator(ThreatList list)
        {
            _internalIndices = list._internalThreats;
            _externalIndices = list._externalThreats;
            _internalIndex = 0;
            _externalIndex = 0;
        }

        public bool MoveNext()
        {
            if (_internalIndex >= _internalIndices.Count)
            {
                if (_externalIndex >= _externalIndices.Count)
                    return false;

                Current = _externalIndices[_externalIndex];
                _externalIndex++;
                return true;
            }

            if (_externalIndex >= _externalIndices.Count)
            {
                Current = _internalIndices[_internalIndex];
                _internalIndex++;
                return true;
            }

            if (_internalIndices[_internalIndex] < _externalIndices[_externalIndex])
            {
                Current = _internalIndices[_internalIndex];
                _internalIndex++;
            }
            else
            {
                Current = _externalIndices[_externalIndex];
                _externalIndex++;
            }
            return true;
        }
    }

    public struct ReverseEnumerator
    {
        private readonly List<int> _internalIndices;
        private readonly List<int> _externalIndices;
        private int _internalIndex;
        private int _externalIndex;

        public int Current { get; private set; }

        internal ReverseEnumerator(ThreatList list)
        {
            _internalIndices = list._internalThreats;
            _externalIndices = list._externalThreats;
            _internalIndex = _internalIndices.Count - 1;
            _externalIndex = _externalIndices.Count - 1;
        }

        public bool MoveNext()
        {
            if (_internalIndex < 0)
            {
                if (_externalIndex < 0)
                    return false;

                Current = _externalIndices[_externalIndex];
                _externalIndex--;
                return true;
            }

            if (_externalIndex < 0)
            {
                Current = _internalIndices[_internalIndex];
                _internalIndex--;
                return true;
            }

            if (_internalIndices[_internalIndex] < _externalIndices[_externalIndex])
            {
                Current = _externalIndices[_externalIndex];
                _externalIndex--;
            }
            else
            {
                Current = _internalIndices[_internalIndex];
                _internalIndex--;
            }
            return true;
        }

        public ReverseEnumerator GetEnumerator() => this;
    }
}
