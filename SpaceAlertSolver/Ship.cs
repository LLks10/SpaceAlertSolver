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
    public const int NUM_DEFECTS = 6;

    public readonly Game Game;

    public Player[] Players => Game.players;
    public AndroidState AndroidTopRight;
    public AndroidState AndroidBottomLeft;
    public readonly int[] Shields = new int[3];
    public readonly int[] ShieldsCap = new int[3];
    public readonly int[] Reactors = new int[3];
    public readonly int[] ReactorsCap = new int[3];
    public readonly int[] LaserDamage = new int[3];
    public readonly int[] PlasmaDamage = new int[3];
    public readonly int[] BDefect = new int[6];
    public readonly int[] CDefect = new int[7];
    public readonly int[] StationStatus = new int[6];
    public readonly int[] Damage = new int[3];
    public readonly bool[] Fissured = new bool[3];

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
        other.LaserDamage.CopyTo(LaserDamage, 0);
        other.PlasmaDamage.CopyTo(PlasmaDamage, 0);
        other.BDefect.CopyTo(BDefect, 0);
        other.CDefect.CopyTo(CDefect, 0);
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
        LaserDamage[0] = LaserDamage[2] = 4;
        LaserDamage[1] = 5;
        Array.Fill(PlasmaDamage, 2);
        Array.Fill(BDefect, 0);
        Array.Fill(CDefect, 0);
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

    internal ref Player GetCurrentTurnPlayer()
    {
        return ref Game.GetCurrentTurnPlayer();
    }

    public void DealDamage(int zone, int amount)
    {
        amount += Game.ScoutBonus;
        Shields[zone] -= amount;
        int dmg = -Shields[zone];

        if (Fissured[zone])
            dmg *= 2;

        //Excess damage
        if (Shields[zone] < 0)
        {
            ApplyDamage(zone, dmg);
            Shields[zone] = 0;
        }
    }

    public void DealDamageIntern(int zone, int amount)
    {
        if (Fissured[zone])
            amount *= 2;

        ApplyDamage(zone, amount);
    }

    void ApplyDamage(int zone, int amount)
    {
        if(amount > 0)
        {
            NumUndeterminedDefects[zone] += amount;
            SetUndeterminedDefects(zone);
            Damage[zone] += amount; // add damage
        }
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
                PlasmaDamage[zone]--;
                break;
            case Defects.weapontop:
                LaserDamage[zone]--;
                break;
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

    public void OnTurnStart()
    {
        _cannonFired = 0;
        _liftUsed = _liftReset;
    }

    public void OnTurnEnd()
    {
        RocketReady = _rocketFired;
        _rocketFired = false;
    }

    public bool CanFireCannon(int position)
    {
        return (_cannonFired & (1 << position)) == 0;
    }

    public void FireCannon(int position)
    {
        _cannonFired |= (1 << position);
    }

    public bool LiftWillDelay(int zone)
    {
        return (_liftUsed & (1 << zone)) != 0;
    }

    public void UseLift(int zone)
    {
        _liftUsed |= (1 << zone);
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
