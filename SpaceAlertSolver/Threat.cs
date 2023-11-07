namespace SpaceAlertSolver;

internal partial struct Threat
{
    public int Health, Shield, Damage, Speed, Distance, ScoreWin, ScoreLose;
    public bool RocketImmune, Alive, Beaten;
    public readonly bool IsExternal;
    public int Zone;
    public IGame Game = null!;
    public Trajectory Trajectory = null!;

    private delegate void ActDelegate(ref Threat @this);
    private ActDelegate _actX;
    private ActDelegate _actY;
    private ActDelegate _actZ;

    private int _value1;

    /// <summary>
    /// External threat constructor
    /// </summary>
    private Threat(int health, int shield, int speed, int scoreWin, int scoreLose, bool rocketImmune = false,
        ActDelegate? actX = null, ActDelegate? actY = null, ActDelegate? actZ = null)
    {
        _actX = actX!; // if it's null if will be solved externally
        _actY = actY!;
        _actZ = actZ!;

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
        _actX ??= GetType().GetMethod($"{name}ActX")!.CreateDelegate<ActDelegate>();
        _actY ??= GetType().GetMethod($"{name}ActY")!.CreateDelegate<ActDelegate>();
        _actZ ??= GetType().GetMethod($"{name}ActZ")!.CreateDelegate<ActDelegate>();
    }

    public void ActX() => _actX(ref this);
    public void ActY() => _actY(ref this);
    public void ActZ() => _actZ(ref this);
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
