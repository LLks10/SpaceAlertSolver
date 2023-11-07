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

    public readonly bool IsInitialized => _actZ != null;

    private delegate void ActDelegate(ref Threat @this);
    private readonly ActDelegate _actX;
    private readonly ActDelegate _actY;
    private readonly ActDelegate _actZ;

    private int _value1;

    private Threat(ActDelegate actX, ActDelegate actY, ActDelegate actZ,
        int health, int shield, int speed, int scoreWin, int scoreLose, bool rocketImmune = false)
    {
        _actX = actX;
        _actY = actY;
        _actZ = actZ;

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

    public void ActX() => _actX(ref this);
    public void ActY() => _actY(ref this);
    public void ActZ() => _actZ(ref this);
}

[AttributeUsage(AttributeTargets.Method)]
internal abstract class ThreatIdAttribute : Attribute
{
    public int ThreatId;

    public ThreatIdAttribute(int threatId)
    {
        ThreatId = threatId;
    }
}

internal sealed class InternalCommonThreatAttribute : ThreatIdAttribute
{
    public InternalCommonThreatAttribute(int threatId) : base(threatId) { }
}

internal sealed class InternalSevereThreatAttribute : ThreatIdAttribute
{
    public InternalSevereThreatAttribute(int threatId) : base(threatId) { }
}

internal sealed class ExternalCommonThreatAttribute : ThreatIdAttribute
{
    public ExternalCommonThreatAttribute(int threatId) : base(threatId) { }
}

internal sealed class ExternalSevereThreatAttribute : ThreatIdAttribute
{
    public ExternalSevereThreatAttribute(int threatId) : base(threatId) { }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal class ThreatNameAttribute : Attribute
{
    public string Name;

    public ThreatNameAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal sealed class PrimaryThreatNameAttribute : ThreatNameAttribute
{
    public PrimaryThreatNameAttribute(string name) : base(name) { }
}
