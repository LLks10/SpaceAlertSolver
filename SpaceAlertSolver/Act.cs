using System.Diagnostics;

namespace SpaceAlertSolver;

public enum Act
{
    Empty,
    Right,
    Left,
    Lift,
    A,
    B,
    C,
    Fight,
    HeroicTopLeft,
    HeroicTopMiddle,
    HeroicTopRight,
    HeroicDownLeft,
    HeroicDownMiddle,
    HeroicDownRight,
    HeroicA,
    HeroicB,
    HeroicFight
}

public static class ActUtils
{
    public static string ToStr(this Act a) => a switch
    {
        Act.Left => "Red",
        Act.Right => "Blue",
        Act.Lift => "Lift",
        Act.A => "A",
        Act.B => "B",
        Act.C => "C",
        Act.Fight => "Robot",
        Act.Empty => "-",
        Act.HeroicTopLeft => "HTRed",
        Act.HeroicTopMiddle => "HTWht",
        Act.HeroicTopRight => "HTBlu",
        Act.HeroicDownLeft => "HDRed",
        Act.HeroicDownMiddle => "HDWht",
        Act.HeroicDownRight => "HDBlu",
        Act.HeroicA => "HA",
        Act.HeroicB => "HB",
        Act.HeroicFight => "HRbt",
        _ => throw new UnreachableException("Whole enum should be covered"),
    };

    public static string Pad(this string str, int length)
    {
        int missingLength = length - str.Length;
        int numSpacesLeft = missingLength / 2;

        return str.PadLeft(numSpacesLeft + str.Length).PadRight(length);
    }

    public static string ActArrayToString(Act[] acts)
    {
        char[] result = new char[acts.Length];
        for (int i = 0; i < acts.Length; i++)
        {
            result[i] = acts[i] switch
            {
                Act.Empty => ' ',
                Act.Right => 'r',
                Act.Left => 'e',
                Act.Lift => 'd',
                Act.A => 'a',
                Act.B => 'b',
                Act.C => 'c',
                Act.Fight => 'f',
                Act.HeroicTopLeft => '1',
                Act.HeroicTopMiddle => '2',
                Act.HeroicTopRight => '3',
                Act.HeroicDownLeft => '4',
                Act.HeroicDownMiddle => '5',
                Act.HeroicDownRight => '6',
                Act.HeroicA => 'A',
                Act.HeroicB => 'B',
                Act.HeroicFight => 'F',
                _ => throw new UnreachableException(),
            };
        }
        return new string(result);
    }

    public static Act[] ParseActionsFromString(string str)
    {
        const int NUM_ACTIONS = 12;

        Act[] actions = new Act[NUM_ACTIONS];
        for (int i = 0; i < NUM_ACTIONS; i++)
        {
            actions[i] = str[i] switch
            {
                'a' => Act.A,
                'b' => Act.B,
                'c' => Act.C,
                'f' => Act.Fight,
                'r' => Act.Right,
                'e' => Act.Left,
                'd' => Act.Lift,
                '1' => Act.HeroicTopLeft,
                '2' => Act.HeroicTopMiddle,
                '3' => Act.HeroicTopRight,
                '4' => Act.HeroicDownLeft,
                '5' => Act.HeroicDownMiddle,
                '6' => Act.HeroicDownRight,
                'A' => Act.HeroicA,
                'B' => Act.HeroicB,
                'F' => Act.HeroicFight,
                ' ' => Act.Empty,
                _ => throw new ArgumentException("Unrecognized action character"),
            };
        }
        return actions;
    }
}
