using System.Diagnostics;

namespace SpaceAlertSolver;

internal partial struct Threat
{
    public int Health, Shield, Damage, Speed, Distance, ScoreWin, ScoreLose;
    public bool Alive, Beaten;
    public readonly bool IsExternal;
    public int Zone;
    public IGame Game = null!;
    public Trajectory Trajectory = null!;

    private delegate void SimpleDelegate(ref Threat @this);
    private SimpleDelegate _actX;
    private SimpleDelegate _actY;
    private SimpleDelegate _actZ;
    private SimpleDelegate _onBeaten;

    private delegate int DistanceDelegate(ref Threat @this, DamageSource damageSource);
    private DistanceDelegate _getDistance;

    private delegate void DamageDelegate(ref Threat @this, DamageSource damageSource, int damage);
    private DamageDelegate _dealDamage;

    private int _value1;

    /// <summary>
    /// External threat constructor
    /// </summary>
    private Threat(int health, int shield, int speed, int scoreWin, int scoreLose,
        SimpleDelegate? actX = null, SimpleDelegate? actY = null, SimpleDelegate? actZ = null, SimpleDelegate? onBeaten = null,
        DistanceDelegate? getDistance = null, DamageDelegate? dealDamage = null)
    {
        _actX = actX!; // if it's null if will be solved externally
        _actY = actY!;
        _actZ = actZ!;
        _onBeaten = onBeaten!;
        _getDistance = getDistance!;
        _dealDamage = dealDamage!;

        IsExternal = true;
        // Zone is set externally
        Health = health;
        Shield = shield;
        Damage = 0;
        Speed = speed;
        // Distance is set externally
        ScoreWin = scoreWin;
        ScoreLose = scoreLose;
        Alive = true;
        Beaten = false;
    }

    public void InitializeUndefinedDelegates(string name)
    {
        if (_actX == null)
        {
            SimpleDelegate? method = GetType().GetMethod($"{name}ActX")?.CreateDelegate<SimpleDelegate>();
            Debug.Assert(method != null, "Cannot find ActX method, nor is it set in the Create method");
            _actX = method;
        }
        if (_actY == null)
        {
            SimpleDelegate? method = GetType().GetMethod($"{name}ActY")?.CreateDelegate<SimpleDelegate>();
            Debug.Assert(method != null, "Cannot find ActY method, nor is it set in the Create method");
            _actY = method;
        }
        if (_actZ == null)
        {
            SimpleDelegate? method = GetType().GetMethod($"{name}ActZ")?.CreateDelegate<SimpleDelegate>();
            Debug.Assert(method != null, "Cannot find ActZ method, nor is it set in the Create method");
            _actZ = method;
        }

        if (_onBeaten == null)
        {
            SimpleDelegate? method = GetType().GetMethod($"{name}OnBeaten")?.CreateDelegate<SimpleDelegate>();
            if (method == null)
                _onBeaten = Blank;
            else
                _onBeaten = method;
        }

        if (_getDistance == null)
        {
            DistanceDelegate? method = GetType().GetMethod($"{name}GetDistance")?.CreateDelegate<DistanceDelegate>();
            if (method == null)
                _getDistance = DefaultGetDistance;
            else
                _getDistance = method;
        }

        if (_dealDamage == null)
        {
            DamageDelegate? method = GetType().GetMethod($"{name}DealDamage")?.CreateDelegate<DamageDelegate>();
            if (method == null)
                _dealDamage = DefaultDealDamage;
            else
                _dealDamage = method;
        }
    }

    public void ActX() => _actX(ref this);
    public void ActY() => _actY(ref this);
    public void ActZ() => _actZ(ref this);
    public void OnBeaten() => _onBeaten(ref this);
    public int GetDistance(DamageSource damageSource) => _getDistance(ref this, damageSource);
    public void DealDamage(DamageSource damageSource, int damage) => _dealDamage(ref this, damageSource, damage);

    private static void Blank(ref Threat _) { }
    private static int DefaultGetDistance(ref Threat @this, DamageSource _)
    {
        Debug.Assert(!@this.Beaten, "Threat should not be beaten when this method is called");
        return @this.Distance;
    }

    private static void DefaultDealDamage(ref Threat @this, DamageSource _, int damage)
    {
        @this.Damage += damage;
    }

    private static int RocketImmuneGetDistance(ref Threat @this, DamageSource damageSource)
    {
        if (damageSource == DamageSource.Rocket)
            return int.MaxValue;
        return DefaultGetDistance(ref @this, damageSource);
    }
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

internal enum DamageSource
{
    HeavyLaserCannon,
    PlasmaCannon,
    PulseCannon,
    Rocket,
}
