using System.Diagnostics;

namespace SpaceAlertSolver;

internal partial struct Threat
{
    public int Health, Shield, Damage, Speed, Distance, ScoreWin, ScoreLose;
    public bool RocketImmune, Alive, Beaten;
    public readonly bool IsExternal;
    public int Zone;
    public IGame Game = null!;
    public Trajectory Trajectory = null!;

    private delegate void ThreatDelegate(ref Threat @this);
    private ThreatDelegate _actX;
    private ThreatDelegate _actY;
    private ThreatDelegate _actZ;
    private ThreatDelegate _onBeaten;

    private int _value1;

    /// <summary>
    /// External threat constructor
    /// </summary>
    private Threat(int health, int shield, int speed, int scoreWin, int scoreLose, bool rocketImmune = false,
        ThreatDelegate? actX = null, ThreatDelegate? actY = null, ThreatDelegate? actZ = null, ThreatDelegate? onBeaten = null)
    {
        _actX = actX!; // if it's null if will be solved externally
        _actY = actY!;
        _actZ = actZ!;
        _onBeaten = onBeaten!;

        IsExternal = true;
        // Zone is set externally
        Health = health;
        Shield = shield;
        Damage = 0;
        Speed = speed;
        // Distance is set externally
        ScoreWin = scoreWin;
        ScoreLose = scoreLose;
        RocketImmune = rocketImmune;
        Alive = true;
        Beaten = false;
    }

    public void InitializeUndefinedDelegates(string name)
    {
        if (_actX == null)
        {
            ThreatDelegate? method = GetType().GetMethod($"{name}ActX")?.CreateDelegate<ThreatDelegate>();
            Debug.Assert(method != null, "Cannot find ActX method, nor is it set in the Create method");
            _actX = method;
        }
        if (_actY == null)
        {
            ThreatDelegate? method = GetType().GetMethod($"{name}ActY")?.CreateDelegate<ThreatDelegate>();
            Debug.Assert(method != null, "Cannot find ActY method, nor is it set in the Create method");
            _actY = method;
        }
        if (_actZ == null)
        {
            ThreatDelegate? method = GetType().GetMethod($"{name}ActZ")?.CreateDelegate<ThreatDelegate>();
            Debug.Assert(method != null, "Cannot find ActZ method, nor is it set in the Create method");
            _actZ = method;
        }

        if (_onBeaten == null)
        {
            ThreatDelegate? method = GetType().GetMethod($"{name}OnBeaten")?.CreateDelegate<ThreatDelegate>();
            if (method == null)
                _onBeaten = Blank;
            else
                _onBeaten = method;
        }
    }

    public void ActX() => _actX(ref this);
    public void ActY() => _actY(ref this);
    public void ActZ() => _actZ(ref this);
    public void OnBeaten() => _onBeaten(ref this);

    private static void Blank(ref Threat _) { }
}

[AttributeUsage(AttributeTargets.Method)]
internal abstract class CreateThreatAttribute : Attribute
{
    public string[] Names;

    public CreateThreatAttribute(params string[] names)
    {
        Names = names;
    }
}

internal sealed class InternalCommonThreatAttribute : CreateThreatAttribute
{
    public InternalCommonThreatAttribute(params string[] names) : base(names) { }
}

internal sealed class InternalSevereThreatAttribute : CreateThreatAttribute
{
    public InternalSevereThreatAttribute(params string[] names) : base(names) { }
}

internal sealed class ExternalCommonThreatAttribute : CreateThreatAttribute
{
    public ExternalCommonThreatAttribute(params string[] names) : base(names) { }
}

internal sealed class ExternalSevereThreatAttribute : CreateThreatAttribute
{
    public ExternalSevereThreatAttribute(params string[] names) : base(names) { }
}
