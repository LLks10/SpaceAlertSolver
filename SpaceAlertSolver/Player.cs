using System.Collections.Immutable;

namespace SpaceAlertSolver;

public struct Player
{
    public int Position;
    public int LastActionIndex { get; private set; }
    public bool Alive { get; private set; }
    public bool InIntercept;
    public AndroidState AndroidState;

    private readonly Act[] _actions;
    private int _delayAmount;

    public Player(Player other)
    {
        Position = other.Position;
        LastActionIndex = other.LastActionIndex;
        Alive = other.Alive;
        InIntercept = other.InIntercept;
        AndroidState = other.AndroidState;

        _actions = other._actions.ToArray();
        _delayAmount = other._delayAmount;
    }

    public Player(ImmutableArray<Act> actions)
    {
        Position = 1;
        LastActionIndex = -1;
        Alive = true;
        InIntercept = false;
        AndroidState = AndroidState.None;

        _actions = actions.ToArray();
        _delayAmount = 0;
    }

    public Act GetAction(int index)
    {
        LastActionIndex = index;
        return _actions[index];
    }

    public Act PeekAction(int index)
    {
        return _actions[index];
    }

    public void Kill()
    {
        Alive = false;
        if (AndroidState == AndroidState.Alive)
            AndroidState = AndroidState.Disabled;
    }

    public void Delay(int action)
    {
        if (action >= 11 || _actions[action] == Act.Empty)
        {
            if(action < 12)
                _actions[action] = Act.Empty;
            return;
        }

        //Delay subsequent actions
        if (_actions[action + 1] != Act.Empty)
            Delay(action + 1);

        //Move action
        _actions[action + 1] = _actions[action];
        _actions[action] = Act.Empty;
    }
}

public enum AndroidState
{
    None,
    Alive,
    Disabled,
}
