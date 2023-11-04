using System.Collections.Immutable;
using System.Diagnostics;

namespace SpaceAlertSolver;

public struct Player
{
    public Position Position { get; private set; }
    public bool Alive { get; private set; }
    public bool InIntercept;
    public AndroidState AndroidState;

    private readonly ImmutableArray<Act> _actions;
    private int _nextActionIndex;
    private int _delayAmount;
    private bool _delayed;
    private bool _lastActionWasEmpty;

    public Player(ImmutableArray<Act> actions)
    {
        Position = Position.TopMiddle;
        Alive = true;
        InIntercept = false;
        AndroidState = AndroidState.None;

        _actions = actions;
        _nextActionIndex = 0;
        _delayAmount = 0;
        _delayed = false;
        _lastActionWasEmpty = true;
    }

    public Act GetNextAction()
    {
        if (_delayed)
        {
            _nextActionIndex++;
            _delayAmount++;
            _delayed = false;
            _lastActionWasEmpty = true;
            return Act.Empty;
        }

        Act act = PeekNextAction();
        _nextActionIndex++;
        _lastActionWasEmpty = act == Act.Empty;
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
        if (_lastActionWasEmpty)
            return;

        if (_delayed)
            _delayed = false;
        _delayAmount++;
        _lastActionWasEmpty = true;
    }

    public void TryMoveLeft() => Position = Position.GetLeft();

    public void TryMoveRight() => Position = Position.GetRight();

    public void TryTakeElevator() => Position = Position.GetElevator();

    public void MoveToSpace() => Position = Position.Space;

    public void ReturnFromSpace()
    {
        Debug.Assert(Position == Position.Space);
        Position = Position.TopLeft;
    }
}

public enum AndroidState
{
    None,
    Alive,
    Disabled,
}
