using System.Security.Cryptography.X509Certificates;

namespace SpaceAlertSolver;

//Position
//0  1  2
//3  4  5   6-> outside

//1  2  4
//8 16 32   64 Bitmasks stations

//1  2  4  Bitmasks zone
public sealed class Ship
{
    public const int NUM_DEFECTS = 6;

    public readonly Game game;

    public Player[] players => game.players;
    public AndroidState AndroidTopRight;
    public AndroidState AndroidBottomLeft;
    public readonly int[] shields = new int[3];
    public readonly int[] shieldsCap = new int[3];
    public readonly int[] reactors = new int[3];
    public readonly int[] reactorsCap = new int[3];
    public readonly int[] laserDamage = new int[3];
    public readonly int[] plasmaDamage = new int[3];
    public readonly int[] BDefect = new int[6];
    public readonly int[] CDefect = new int[7];
    public readonly int[] stationStatus = new int[6];
    public readonly int[] damage = new int[3];
    public readonly bool[] fissured = new bool[3];

    //Bit flags
    public int liftUsed, liftReset;
    public int cannonFired;

    public int capsules;

    public int rockets;
    public bool rocketFired;
    public bool rocketReady;

    public bool interceptorReady;

    public int scoutBonus;

    //Defects
    readonly int[] numUndeterminedDefects = new int[3]; // the number of undetermined defects in each zone
    readonly int[] numDefectOptions = new int[3]; // the number of places that a defect can be
    public readonly DefectState[,] defectStates = new DefectState[3,NUM_DEFECTS]; // the state of each defect in each zone
                                                                                  // defect: it is definitely broken
                                                                                  // notDefect: it is definitely not broken (yet)
                                                                                  // undetermined: it might be broken (this means branch if necessary)

    public Ship(Game game)
    {
        this.game = game;
    }

    public void Init(Ship other)
    {
        AndroidTopRight = other.AndroidTopRight;
        AndroidBottomLeft = other.AndroidBottomLeft;
        other.shields.CopyTo(shields, 0);
        other.shieldsCap.CopyTo(shieldsCap, 0);
        other.reactors.CopyTo(reactors, 0);
        other.reactorsCap.CopyTo(reactorsCap, 0);
        other.laserDamage.CopyTo(laserDamage, 0);
        other.plasmaDamage.CopyTo(plasmaDamage, 0);
        other.BDefect.CopyTo(BDefect, 0);
        other.CDefect.CopyTo(CDefect, 0);
        other.stationStatus.CopyTo(stationStatus, 0);
        other.damage.CopyTo(damage, 0);
        other.fissured.CopyTo(fissured, 0);

        liftUsed = other.liftUsed;
        liftReset = other.liftReset;
        cannonFired = other.cannonFired;
        capsules = other.capsules;
        rockets = other.rockets;
        rocketFired = other.rocketFired;
        rocketReady = other.rocketReady;
        interceptorReady = other.interceptorReady;
        scoutBonus = other.scoutBonus;

        other.numUndeterminedDefects.CopyTo(numUndeterminedDefects, 0);
        other.numDefectOptions.CopyTo(numDefectOptions, 0);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < NUM_DEFECTS; j++)
            {
                defectStates[i,j] = other.defectStates[i,j];
            }
        }
    }

    public void Init()
    {
        AndroidTopRight = AndroidState.Alive;
        AndroidBottomLeft = AndroidState.Alive;
        Array.Fill(shields, 1);
        shieldsCap[0] = shieldsCap[2] = 2;
        shieldsCap[1] = 3;
        reactors[0] = reactors[2] = 2;
        reactors[1] = 3;
        reactorsCap[0] = reactorsCap[2] = 3;
        reactorsCap[1] = 5;
        laserDamage[0] = laserDamage[2] = 4;
        laserDamage[1] = 5;
        Array.Fill(plasmaDamage, 2);
        Array.Fill(BDefect, 0);
        Array.Fill(CDefect, 0);
        Array.Fill(stationStatus, 0);
        Array.Fill(damage, 0);
        Array.Fill(fissured, false);

        liftUsed = 0;
        liftReset = 0;
        capsules = 3;
        rockets = 3;
        rocketFired = false;
        rocketReady = false;
        interceptorReady = true;
        scoutBonus = 0;

        //Setup defects
        Array.Fill(numUndeterminedDefects, 0);
        Array.Fill(numDefectOptions, 0);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < NUM_DEFECTS; j++)
            {
                defectStates[i,j] = DefectState.notDefect;
            }
        }
    }

    internal ref Player GetCurrentTurnPlayer()
    {
        return ref game.GetCurrentTurnPlayer();
    }

    public void DealDamage(int zone, int amount)
    {
        amount += scoutBonus;
        shields[zone] -= amount;
        int dmg = -shields[zone];

        if (fissured[zone])
            dmg *= 2;

        //Excess damage
        if (shields[zone] < 0)
        {
            ApplyDamage(zone, dmg);
            shields[zone] = 0;
        }
    }

    public void DealDamageIntern(int zone, int amount)
    {
        if (fissured[zone])
            amount *= 2;

        ApplyDamage(zone, amount);
    }

    void ApplyDamage(int zone, int amount)
    {
        if(amount > 0)
        {
            numUndeterminedDefects[zone] += amount;
            SetUndeterminedDefects(zone);
            damage[zone] += amount; // add damage
        }
    }

    void SetUndeterminedDefects(int zone)
    {
        for (int i = 0; i < NUM_DEFECTS; i++)
        {
            if (defectStates[zone,i] == DefectState.notDefect)
            {
                defectStates[zone,i] = DefectState.undetermined; // it might be broken now
                numDefectOptions[zone]++;
            }
        }
    }

    void RemoveUndeterminedDefects(int zone)
    {
        for (int i = 0; i < NUM_DEFECTS; i++)
        {
            if (defectStates[zone,i] == DefectState.undetermined)
            {
                defectStates[zone,i] = DefectState.notDefect;
                numDefectOptions[zone]--;
            }
        }
    }

    public void AddDefect(int zone, Defects defect)
    { // we may not pass a defect that was not undetermined
        defectStates[zone,(int)defect] = DefectState.defect;
        numDefectOptions[zone]--;

        numUndeterminedDefects[zone]--;
        if (numUndeterminedDefects[zone] == 0)
        {
            RemoveUndeterminedDefects(zone);
        }

        switch (defect)
        {
            case Defects.lift:
                liftReset |= 1 << zone;
                liftUsed |= 1 << zone;
                break;
            case Defects.reactor:
                reactorsCap[zone]--;
                reactors[zone] = Math.Min(reactors[zone], reactorsCap[zone]);
                break;
            case Defects.shield:
                shieldsCap[zone]--;
                shields[zone] = Math.Min(shields[zone], shieldsCap[zone]);
                break;
            case Defects.weaponbot:
                plasmaDamage[zone]--;
                break;
            case Defects.weapontop:
                laserDamage[zone]--;
                break;
        }
    }

    public void NotDefect(int zone, Defects defect)
    { // we may not pass a defect that was not undetermined
        defectStates[zone,(int)defect] = DefectState.notDefect;
        numDefectOptions[zone]--;
    }

    public double ChanceOfDefect(int zone)
    {
        return (double)numUndeterminedDefects[zone] / numDefectOptions[zone];
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
