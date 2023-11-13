using System.Diagnostics;

namespace SpaceAlertSolver;

public partial struct Threat
{
    public readonly int MaxHealth;
    public int Health, Shield, Damage, Speed, Distance, ScoreWin, ScoreLose;
    public int Inaccessibility
    {
        get => Shield;
        set => Shield = value;
    }
    public bool Alive, Beaten;
    public readonly bool IsExternal;
    public int Zone;
    public IGame Game = null!;
    public Position Position;

    private delegate void SimpleDelegate(ref Threat @this);
    private SimpleDelegate _actX;
    private SimpleDelegate _actY;
    private SimpleDelegate _actZ;
    private SimpleDelegate _onBeaten;
    private SimpleDelegate _processDamage;

    private delegate bool TargetDelegate(ref Threat @this, DamageSource damageSource, Position position);
    private TargetDelegate? _isTargetedBy;

    private delegate int DistanceDelegate(ref Threat @this, DamageSource damageSource);
    private DistanceDelegate _getDistance;

    private delegate void ExternalDamageDelegate(ref Threat @this, DamageSource damageSource, int damage);
    private ExternalDamageDelegate? _dealExternalDamage;

    private delegate void InternalDamageDelegate(ref Threat @this, DamageSource damageSourceType, int damage, int playerId, Position position);
    private InternalDamageDelegate? _dealInternalDamage;

    private int _value1;

    /// <summary>
    /// External threat constructor
    /// </summary>
    private Threat(int health, int shield, int speed, int scoreWin, int scoreLose,
        SimpleDelegate? actX = null, SimpleDelegate? actY = null, SimpleDelegate? actZ = null, SimpleDelegate? onBeaten = null,
        SimpleDelegate? processDamage = null, DistanceDelegate? getDistance = null, ExternalDamageDelegate? dealDamage = null)
    {
        _actX = actX!; // if it's null if will be solved externally
        _actY = actY!;
        _actZ = actZ!;
        _onBeaten = onBeaten!;
        _processDamage = processDamage!;
        _getDistance = getDistance!;
        _dealExternalDamage = dealDamage;
        _dealInternalDamage = null;
        _isTargetedBy = null;

        Position = Position.Space;
        IsExternal = true;
        // Zone is set externally
        MaxHealth = health;
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

    /// <summary>
    /// Internal threat constructor
    /// </summary>
    private Threat(int health, int speed, Position position, int scoreWin, int scoreLose, TargetDelegate? isTargetedBy = null,
        SimpleDelegate ? actX = null, SimpleDelegate? actY = null, SimpleDelegate? actZ = null, int inaccessibility = 0,
        SimpleDelegate? onBeaten = null,SimpleDelegate? processDamage = null, DistanceDelegate? getDistance = null, InternalDamageDelegate? dealDamage = null)
    {
        _actX = actX!;
        _actY = actY!;
        _actZ = actZ!;
        _onBeaten = onBeaten!;
        _processDamage = processDamage!;
        _getDistance = getDistance!;
        _dealExternalDamage = null;
        _dealInternalDamage = dealDamage;
        _isTargetedBy = isTargetedBy;

        IsExternal = false;
        Health = health;
        MaxHealth = health;
        Inaccessibility = inaccessibility;
        Position = position;
        Damage = 0;
        Speed = speed;
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
        if (!IsExternal && _isTargetedBy == null)
        {
            TargetDelegate? method = GetType().GetMethod($"{name}IsTargetedBy")?.CreateDelegate<TargetDelegate>();
            Debug.Assert(method != null, "Cannot find IsTargetedBy method, nor is it set in the Create method");
            _isTargetedBy = method;
        }

        if (_onBeaten == null)
        {
            SimpleDelegate? method = GetType().GetMethod($"{name}OnBeaten")?.CreateDelegate<SimpleDelegate>();
            if (method == null)
                _onBeaten = Blank;
            else
                _onBeaten = method;
        }

        if (_processDamage == null)
        {
            SimpleDelegate? method = GetType().GetMethod($"{name}ProcessDamage")?.CreateDelegate<SimpleDelegate>();
            if (method == null)
            {
                if (IsExternal)
                    _processDamage = DefaultExternalProcessDamage;
                else
                    _processDamage = Blank;
            }
            else
            {
                _processDamage = method;
            }
        }

        if (_getDistance == null)
        {
            DistanceDelegate? method = GetType().GetMethod($"{name}GetDistance")?.CreateDelegate<DistanceDelegate>();
            if (method == null)
                _getDistance = DefaultGetDistance;
            else
                _getDistance = method;
        }

        if (IsExternal && _dealExternalDamage == null)
        {
            ExternalDamageDelegate? method = GetType().GetMethod($"{name}DealDamage")?.CreateDelegate<ExternalDamageDelegate>();
            if (method == null)
                _dealExternalDamage = DefaultExternalDealDamage;
            else
                _dealExternalDamage = method;
        }

        if (!IsExternal && _dealInternalDamage == null)
        {
            InternalDamageDelegate? method = GetType().GetMethod($"{name}DealDamage")?.CreateDelegate<InternalDamageDelegate>();
            if (method == null)
                _dealInternalDamage = DefaultInternalDealDamage;
            else
                _dealInternalDamage = method;
        }
    }

    public void ActX() => _actX(ref this);
    public void ActY() => _actY(ref this);
    public void ActZ() => _actZ(ref this);
    public void OnBeaten() => _onBeaten(ref this);
    public void ProcessDamage() => _processDamage(ref this);
    public int GetDistance(DamageSource damageSource) => _getDistance(ref this, damageSource);
    public void DealExternalDamage(DamageSource damageSource, int damage) => _dealExternalDamage!(ref this, damageSource, damage);
    public void DealInternalDamage(DamageSource damageSource, int damage, int playerId, Position position) => _dealInternalDamage!(ref this, damageSource, damage, playerId, position);

    private static SimpleDelegate ActDealDamage(int damage)
    {
        return (ref Threat @this) => { @this.Game.DealExternalDamage(@this.Zone, damage); };
    }

    private static SimpleDelegate ActDealDamageAll(int damage)
    {
        return (ref Threat @this) =>
        {
            @this.Game.DealExternalDamage(0, damage);
            @this.Game.DealExternalDamage(1, damage);
            @this.Game.DealExternalDamage(2, damage);
        };
    }

    public bool UpdateAlive()
    {
        if (Health <= 0)
        {
            Beaten = true;
            Alive = false;
        }
        return Alive;
    }

    private static void Blank(ref Threat _) { }
    private static int DefaultGetDistance(ref Threat @this, DamageSource _)
    {
        Debug.Assert(!@this.Beaten, "Threat should not be beaten when this method is called");
        return @this.Distance;
    }

    private static void DestroyShip(ref Threat @this)
    {
        @this.Game.DestroyShip();
    }

    private static void DefaultExternalDealDamage(ref Threat @this, DamageSource _, int damage)
    {
        @this.Damage += damage;
    }

    private static void DefaultInternalDealDamage(ref Threat @this, DamageSource _, int damage, int __, Position ___)
    {
        @this.Health -= damage;
        @this.UpdateAlive();
    }

    private static void InternalDealDamageFightback(ref Threat @this, DamageSource damageSource, int damage, int playerId, Position position)
    {
        DefaultInternalDealDamage(ref @this, damageSource, damage, playerId, position);
        Debug.Assert(@this.Game.Players[playerId].AndroidState == AndroidState.Alive, "Expecting alive androids when fighting back");
        @this.Game.Players[playerId].AndroidState = AndroidState.Disabled;
    }

    private static void DefaultExternalProcessDamage(ref Threat @this)
    {
        Debug.Assert(@this.Damage > 0, "Should only process damage when there is some");
        int resultingDamage = @this.Damage - @this.Shield;
        @this.Damage = 0;
        if (resultingDamage <= 0)
            return;

        @this.Health -= resultingDamage;
        @this.UpdateAlive();
    }

    private static int RocketImmuneGetDistance(ref Threat @this, DamageSource damageSource)
    {
        if (damageSource == DamageSource.Rocket)
            return int.MaxValue;
        return DefaultGetDistance(ref @this, damageSource);
    }

    private static int HeavyLaserImmuneGetDistance(ref Threat @this, DamageSource damageSource)
    {
        if (damageSource == DamageSource.HeavyLaserCannon)
            return int.MaxValue;
        return DefaultGetDistance(ref @this, damageSource);
    }

    private static bool IsTargetedByRobots(ref Threat @this, DamageSource damageSource, Position position)
    {
        return damageSource == DamageSource.Robots && @this.Position == position;
    }
}

[AttributeUsage(AttributeTargets.Method)]
internal abstract class CreateThreatAttribute : Attribute
{
    public readonly string Code;
    public readonly string[] Names;

    public CreateThreatAttribute(string code, params string[] names)
    {
        Code = code;
        Names = names;
    }
}

internal sealed class InternalCommonThreatAttribute : CreateThreatAttribute
{
    public InternalCommonThreatAttribute(string code, params string[] names) : base(code, names) { }
}

internal sealed class InternalSevereThreatAttribute : CreateThreatAttribute
{
    public InternalSevereThreatAttribute(string code, params string[] names) : base(code, names) { }
}

internal sealed class ExternalCommonThreatAttribute : CreateThreatAttribute
{
    public ExternalCommonThreatAttribute(string code, params string[] names) : base(code, names) { }
}

internal sealed class ExternalSevereThreatAttribute : CreateThreatAttribute
{
    public ExternalSevereThreatAttribute(string code, params string[] names) : base(code, names) { }
}

public enum DamageSource
{
    HeavyLaserCannon,
    PlasmaCannon,
    PulseCannon,
    Rocket,
    Interceptors,
    Robots,
    RepairA,
    RepairB,
    RepairC,
}
