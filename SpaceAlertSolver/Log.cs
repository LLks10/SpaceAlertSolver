namespace SpaceAlertSolver;

internal class Log
{
    private static Log? _instance = null;
    public static Log Instance => _instance ?? (_instance = new Log());

    private readonly string _fileName;

    private Log()
    {
        _fileName = $"Log-{DateTime.Now}";
    }

    public void WriteLine(object line)
    {
        string str = line.ToString() ?? "";
        using FileStream fs = new(_fileName, FileMode.Append, FileAccess.Write);
        using StreamWriter sw = new(fs);
        sw.WriteLine(str);
    }
}
