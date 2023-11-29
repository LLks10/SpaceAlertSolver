//#define LOGGING

namespace SpaceAlertSolver;

#if LOGGING
public static class Log
{
    private static string _indent = string.Empty;
    private static FileStream? _fileStream;
    private static StreamWriter? _streamWriter;

    public static void WriteLine(object line)
    {
        if (_fileStream == null || _streamWriter == null)
        {
            string fileName = $"Log-{DateTime.Now}.txt".Replace('/', '-').Replace(':','-');
            _fileStream = new(fileName, FileMode.CreateNew, FileAccess.Write);
            _streamWriter = new(_fileStream);
            _streamWriter.AutoFlush = true;
        }

        string str = line.ToString() ?? "";
        _streamWriter.WriteLine(_indent + str);
        _fileStream.Flush();
    }

    public static void Indent()
    {
        _indent += "| ";
    }

    public static void Dedent()
    {
        _indent = _indent[2..];
    }
}
#else
public static class Log
{
    public static void WriteLine(object _) { }

    public static void Indent() { }

    public static void Dedent() { }
}
#endif
