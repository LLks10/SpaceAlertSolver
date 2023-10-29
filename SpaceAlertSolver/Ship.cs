namespace SpaceAlertSolver;

//Position
//0  1  2
//3  4  5   6-> outside

//1  2  4
//8 16 32   64 Bitmasks stations

//1  2  4  Bitmasks zone
public class Ship
{
    public static int NUM_DEFECTS = 6;

    public Game game;

    public Player[] players;
    public Androids[] androids;
    public int[] shields;
    public int[] shieldsCap;
    public int[] reactors;
    public int[] reactorsCap;
    public int[] laserDamage;
    public int[] plasmaDamage;
    public int[] BDefect, CDefect;
    public int[] stationStatus;
    public int pulseRange;
    public int[] damage;
    public bool[] fissured;

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
    int[] numUndeterminedDefects; // the number of undetermined defects in each zone
    int[] numDefectOptions; // the number of places that a defect can be
    public DefectState[][] defectStates; // the state of each defect in each zone
                                  // defect: it is definitely broken
                                  // notDefect: it is definitely not broken (yet)
                                  // undetermined: it might be broken (this means branch if necessary)

    public Ship(Ship other, Game game, Player[] players)
    {
        this.game = game;
        this.players = players;
        androids = new Androids[2] { new Androids(other.androids[0]), new Androids(other.androids[1]) };
        shields = Extension.CopyArray(other.shields);
        shieldsCap = Extension.CopyArray(other.shieldsCap);
        reactors = Extension.CopyArray(other.reactors);
        reactorsCap = Extension.CopyArray(other.reactorsCap);
        laserDamage = Extension.CopyArray(other.laserDamage);
        plasmaDamage = Extension.CopyArray(other.plasmaDamage);
        BDefect = Extension.CopyArray(other.BDefect);
        CDefect = Extension.CopyArray(other.CDefect);
        stationStatus = Extension.CopyArray(other.stationStatus);
        pulseRange = other.pulseRange;
        damage = Extension.CopyArray(other.damage);
        fissured = Extension.CopyArray(other.fissured);

        liftUsed = other.liftUsed;
        liftReset = other.liftReset;
        cannonFired = other.cannonFired;
        capsules = other.capsules;
        rockets = other.rockets;
        rocketFired = other.rocketFired;
        rocketReady = other.rocketReady;
        interceptorReady = other.interceptorReady;
        scoutBonus = other.scoutBonus;

        numUndeterminedDefects = Extension.CopyArray(other.numUndeterminedDefects);
        numDefectOptions = Extension.CopyArray(other.numDefectOptions);
        defectStates = new DefectState[other.defectStates.Length][];
        for (int i = 0; i < defectStates.Length; i++)
        {
            defectStates[i] = new DefectState[other.defectStates[i].Length];
            for (int j = 0; j < defectStates[i].Length; j++)
            {
                defectStates[i][j] = other.defectStates[i][j];
            }
        }
    }

    public Ship(Game game, Player[] players)
    {
        this.game = game;
        this.players = players;
        //Setup
        shields = new int[] { 1, 1, 1 };
        shieldsCap = new int[] { 2, 3, 2 };
        reactors = new int[] { 2, 3, 2 };
        reactorsCap = new int[] { 3, 5, 3 };
        androids = new Androids[] { new Androids(2), new Androids(3) };
        laserDamage = new int[] { 4, 5, 4 };
        plasmaDamage = new int[] { 2, 2, 2 };
        stationStatus = new int[6];
        BDefect = new int[6];
        CDefect = new int[7];
        fissured = new bool[3];
        damage = new int[] { 0, 0, 0 };
        capsules = 3;
        rockets = 3;
        interceptorReady = true;

        //Setup defects
        numUndeterminedDefects = new int[3] { 0, 0, 0 };
        numDefectOptions = new int[3] { 0, 0, 0 };
        defectStates = new DefectState[3][]; // fill entirety of this with notDefect
        for (int i = 0; i < defectStates.Length; i++)
        {
            defectStates[i] = new DefectState[NUM_DEFECTS];
            for (int j = 0; j < NUM_DEFECTS; j++)
            {
                defectStates[i][j] = DefectState.notDefect;
            }
        }
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
            if (defectStates[zone][i] == DefectState.notDefect)
            {
                defectStates[zone][i] = DefectState.undetermined; // it might be broken now
                numDefectOptions[zone]++;
            }
        }
    }

    void RemoveUndeterminedDefects(int zone)
    {
        for (int i = 0; i < NUM_DEFECTS; i++)
        {
            if (defectStates[zone][i] == DefectState.undetermined)
            {
                defectStates[zone][i] = DefectState.notDefect;
                numDefectOptions[zone]--;
            }
        }
    }

    public void AddDefect(int zone, Defects defect)
    { // we may not pass a defect that was not undetermined
        defectStates[zone][(int)defect] = DefectState.defect;
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
        defectStates[zone][(int)defect] = DefectState.notDefect;
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
