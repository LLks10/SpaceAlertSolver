using System.Diagnostics;

namespace SpaceAlertSolver;

//Position
//0  1  2
//3  4  5   6-> outside

//1  2  4
//8 16 32   64 Bitmasks stations

//1  2  4  Bitmasks zone
internal sealed class Ship
{
    public static readonly CannonStats[] _defaultCannonStats = new CannonStats[6]
    {
        new(DamageSource.HeavyLaserCannon, 4, 3),
        new(DamageSource.HeavyLaserCannon, 5, 3),
        new(DamageSource.HeavyLaserCannon, 4, 3),
        new(DamageSource.PlasmaCannon, 2, 3),
        new(DamageSource.PulseCannon, 1, 2),
        new(DamageSource.PlasmaCannon, 2, 3),
    };

    public const int NUM_DEFECTS = 6;

    public readonly Game Game;

    public AndroidState AndroidTopRight;
    public AndroidState AndroidBottomLeft;
    public readonly int[] Shields = new int[3];
    public readonly int[] ShieldsCap = new int[3];
    public readonly int[] Reactors = new int[3];
    public readonly int[] ReactorsCap = new int[3];
    public readonly int[] StationStatus = new int[6];
    public readonly int[] Damage = new int[3];
    public readonly bool[] Fissured = new bool[3];
    internal readonly CannonStats[] CannonStats = new CannonStats[6];

    private readonly int[] _malfunctionsA = new int[6];
    private readonly int[] _malfunctionsB = new int[6];
    private readonly int[] _malfunctionsC = new int[6];

    //Bit flags
    private int _liftUsed;
    private int _liftReset;
    private int _cannonFired;

    public int CapsulesLeft;

    public int RocketsLeft;
    private bool _rocketFired;
    public bool RocketReady { get; private set; }

    public bool InterceptorReady;

    //Defects
    internal readonly int[] NumUndeterminedDefects = new int[3]; // the number of undetermined defects in each zone
    internal readonly int[] NumDefectOptions = new int[3]; // the number of places that a defect can be
    internal readonly DefectState[,] DefectStates = new DefectState[3,NUM_DEFECTS]; // the state of each defect in each zone
                                                                                  // defect: it is definitely broken
                                                                                  // notDefect: it is definitely not broken (yet)
                                                                                  // undetermined: it might be broken (this means branch if necessary)

    public Ship(Game game)
    {
        this.Game = game;
    }

    public void Init(Ship other)
    {
        AndroidTopRight = other.AndroidTopRight;
        AndroidBottomLeft = other.AndroidBottomLeft;
        other.Shields.CopyTo(Shields, 0);
        other.ShieldsCap.CopyTo(ShieldsCap, 0);
        other.Reactors.CopyTo(Reactors, 0);
        other.ReactorsCap.CopyTo(ReactorsCap, 0);
        other.CannonStats.CopyTo(CannonStats, 0);
        other._malfunctionsA.CopyTo(_malfunctionsA, 0);
        other._malfunctionsB.CopyTo(_malfunctionsB, 0);
        other._malfunctionsC.CopyTo(_malfunctionsC, 0);
        other.StationStatus.CopyTo(StationStatus, 0);
        other.Damage.CopyTo(Damage, 0);
        other.Fissured.CopyTo(Fissured, 0);

        _liftUsed = other._liftUsed;
        _liftReset = other._liftReset;
        _cannonFired = other._cannonFired;
        CapsulesLeft = other.CapsulesLeft;
        RocketsLeft = other.RocketsLeft;
        _rocketFired = other._rocketFired;
        RocketReady = other.RocketReady;
        InterceptorReady = other.InterceptorReady;

        other.NumUndeterminedDefects.CopyTo(NumUndeterminedDefects, 0);
        other.NumDefectOptions.CopyTo(NumDefectOptions, 0);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < NUM_DEFECTS; j++)
            {
                DefectStates[i,j] = other.DefectStates[i,j];
            }
        }
    }

    public void Init()
    {
        AndroidTopRight = AndroidState.Alive;
        AndroidBottomLeft = AndroidState.Alive;
        Array.Fill(Shields, 1);
        ShieldsCap[0] = ShieldsCap[2] = 2;
        ShieldsCap[1] = 3;
        Reactors[0] = Reactors[2] = 2;
        Reactors[1] = 3;
        ReactorsCap[0] = ReactorsCap[2] = 3;
        ReactorsCap[1] = 5;
        _defaultCannonStats.CopyTo(CannonStats, 0);
        Array.Fill(_malfunctionsA, 0);
        Array.Fill(_malfunctionsB, 0);
        Array.Fill(_malfunctionsC, 0);
        Array.Fill(StationStatus, 0);
        Array.Fill(Damage, 0);
        Array.Fill(Fissured, false);

        _liftUsed = 0;
        _liftReset = 0;
        CapsulesLeft = 3;
        RocketsLeft = 3;
        _rocketFired = false;
        RocketReady = false;
        InterceptorReady = true;

        //Setup defects
        Array.Fill(NumUndeterminedDefects, 0);
        Array.Fill(NumDefectOptions, 0);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < NUM_DEFECTS; j++)
            {
                DefectStates[i,j] = DefectState.notDefect;
            }
        }
    }

    public void AddMulfunctionA(Position position) => _malfunctionsA[position.PositionIndex]++;
    public void AddMulfunctionB(Position position) => _malfunctionsB[position.PositionIndex]++;
    public void AddMulfunctionC(Position position) => _malfunctionsC[position.PositionIndex]++;

    public void RemoveMalfunctionA(Position position)
    {
        Debug.Assert(_malfunctionsA[position.PositionIndex] > 0);
        _malfunctionsA[position.PositionIndex]--;
    }

    public void RemoveMalfunctionB(Position position)
    {
        Debug.Assert(_malfunctionsB[position.PositionIndex] > 0);
        _malfunctionsB[position.PositionIndex]--;
    }

    public void RemoveMalfunctionC(Position position)
    {
        Debug.Assert(_malfunctionsC[position.PositionIndex] > 0);
        _malfunctionsC[position.PositionIndex]--;
    }

    public bool HasMalfunctionA(Position position) => _malfunctionsA[position.PositionIndex] > 0;
    public bool HasMalfunctionB(Position position) => _malfunctionsB[position.PositionIndex] > 0;
    public bool HasMalfunctionC(Position position) => _malfunctionsC[position.PositionIndex] > 0;

    public bool DealExternalDamage(int zone, int damage)
    {
        Shields[zone] -= damage;

        //Excess damage
        if (Shields[zone] < 0)
        {
            int dmg = -Shields[zone];
            Shields[zone] = 0;
            return ApplyDamage(zone, dmg);
        }

        return false;
    }

    public bool DealInternalDamage(int zone, int damage) => ApplyDamage(zone, damage);

    bool ApplyDamage(int zone, int damage)
    {
        Debug.Assert(damage > 0);
        if (Fissured[zone])
            damage *= 2;
        NumUndeterminedDefects[zone] += damage;
        SetUndeterminedDefects(zone);
        Damage[zone] += damage; // add damage
        return Damage[zone] >= 7;
    }

    void SetUndeterminedDefects(int zone)
    {
        for (int i = 0; i < NUM_DEFECTS; i++)
        {
            if (DefectStates[zone,i] == DefectState.notDefect)
            {
                DefectStates[zone,i] = DefectState.undetermined; // it might be broken now
                NumDefectOptions[zone]++;
            }
        }
    }

    void RemoveUndeterminedDefects(int zone)
    {
        for (int i = 0; i < NUM_DEFECTS; i++)
        {
            if (DefectStates[zone,i] == DefectState.undetermined)
            {
                DefectStates[zone,i] = DefectState.notDefect;
                NumDefectOptions[zone]--;
            }
        }
    }

    public void AddDefect(int zone, Defects defect)
    { // we may not pass a defect that was not undetermined
        DefectStates[zone,(int)defect] = DefectState.defect;
        NumDefectOptions[zone]--;

        NumUndeterminedDefects[zone]--;
        if (NumUndeterminedDefects[zone] == 0)
        {
            RemoveUndeterminedDefects(zone);
        }

        switch (defect)
        {
            case Defects.lift:
                _liftReset |= 1 << zone;
                _liftUsed |= 1 << zone;
                break;
            case Defects.reactor:
                ReactorsCap[zone]--;
                Reactors[zone] = Math.Min(Reactors[zone], ReactorsCap[zone]);
                break;
            case Defects.shield:
                ShieldsCap[zone]--;
                Shields[zone] = Math.Min(Shields[zone], ShieldsCap[zone]);
                break;
            case Defects.weaponbot:
                {
                    Position pos = Position.GetBottom(zone);
                    if (pos == Position.BottomMiddle)
                        CannonStats[pos.PositionIndex].Range--;
                    else
                        CannonStats[pos.PositionIndex].Damage--;
                    break;
                }
            case Defects.weapontop:
                {
                    Position pos = Position.GetTop(zone);
                    CannonStats[pos.PositionIndex].Damage--;
                    break;
                }
        }
    }

    public void NotDefect(int zone, Defects defect)
    { // we may not pass a defect that was not undetermined
        DefectStates[zone,(int)defect] = DefectState.notDefect;
        NumDefectOptions[zone]--;
    }

    public double ChanceOfDefect(int zone)
    {
        return (double)NumUndeterminedDefects[zone] / NumDefectOptions[zone];
    }

    public bool CanFireRocket()
    {
        return !_rocketFired && RocketsLeft > 0;
    }

    public void FireRocket()
    {
        Debug.Assert(CanFireRocket());
        _rocketFired = true;
        RocketsLeft--;
    }

    public void MoveRockets()
    {
        RocketReady = _rocketFired;
        _rocketFired = false;
    }

    public void OnTurnStart()
    {
        _cannonFired = 0;
        _liftUsed = _liftReset;
    }

    public bool CanFireCannon(Position position, out bool costsEnergy)
    {
        if (position == Position.Space)
        {
            costsEnergy = false;
            return false;
        }

        costsEnergy = position != Position.BottomLeft && position != Position.BottomRight;

        if (costsEnergy && Reactors[position.Zone] <= 0)
            return false;

        return (_cannonFired & (1 << position.PositionIndex)) == 0;
    }

    public void FireCannon(Position position, bool costsEnergy)
    {
        Debug.Assert(CanFireCannon(position, out bool b) && b == costsEnergy);
        _cannonFired |= (1 << position.PositionIndex);
        if (costsEnergy)
            Reactors[position.Zone]--;
    }

    public bool LiftWillDelay(Position position)
    {
        return (_liftUsed & (1 << position.Zone)) != 0;
    }

    public void UseLift(Position position)
    {
        _liftUsed |= (1 << position.Zone);
    }

    public void TryRefillMainReactor()
    {
        if (CapsulesLeft <= 0)
            return;

        CapsulesLeft--;
        Reactors[1] = ReactorsCap[1];
    }
}

public enum Defects
{
    structure,
    lift,
    weapontop,
    weaponbot,
    shield,
    reactor
}

public enum DefectState
{
    defect,
    notDefect,
    undetermined
}

internal record struct CannonStats(DamageSource Type, int Damage, int Range);
