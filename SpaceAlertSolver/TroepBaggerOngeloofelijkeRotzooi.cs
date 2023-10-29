namespace SpaceAlertSolver;

internal static class TroepBaggerOngeloofelijkeRotzooi
{
    public static T[] CopyArray<T>(T[] other)
    {
        T[] output = new T[other.Length];
        Array.Copy(output, other, output.Length);
        return output;
    }
}
