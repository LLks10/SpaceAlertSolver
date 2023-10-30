using System.Collections.Immutable;

namespace SpaceAlertSolver;

public struct Player
{
    public int Position;
    public bool Alive { get; private set; }
    public bool InIntercept;
    public AndroidState AndroidState;

    private readonly ImmutableArray<Act> _actions;
    private int _nextActionIndex;
    private int _delayAmount;
    private bool _delayed;

    public Player(ImmutableArray<Act> actions)
    {
        Position = 1;
        Alive = true;
        InIntercept = false;
        AndroidState = AndroidState.None;

        _actions = actions;
        _nextActionIndex = 0;
        _delayAmount = 0;
        _delayed = false;
    }

    public Act GetNextAction()
    {
        if (_delayed)
        {
            _nextActionIndex++;
            _delayAmount++;
            _delayed = false;
            return Act.Empty;
        }

        Act act = PeekNextAction();
        _nextActionIndex++;
        return act;
    }

    public Act PeekNextAction()
    {
        if (_delayed)
        {
            return Act.Empty;
        }

        Act act = _actions[_nextActionIndex - _delayAmount];
        while (_delayAmount > 0 && act == Act.Empty)
        {
            _delayAmount--;
            act = _actions[_nextActionIndex - _delayAmount];
        }

        return act;
    }

    public void Kill()
    {
        Alive = false;
        if (AndroidState == AndroidState.Alive)
            AndroidState = AndroidState.Disabled;
    }

    public void DelayNext()
    {
        _delayed = true;
    }

    public void DelayCurrent()
    {
        _delayAmount++;
    }
}

public enum AndroidState
{
    None,
    Alive,
    Disabled,
}
