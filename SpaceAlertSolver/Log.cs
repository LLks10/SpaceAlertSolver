//#define LOGGING

namespace SpaceAlertSolver;

#if LOGGING
public static class Log
{
    private static FileStream? _fileStream;
    private static StreamWriter? _streamWriter;

    public static void WriteLine(object line)
    {
        if (_fileStream == null || _streamWriter == null)
        {
            string fileName = $"Log-{DateTime.Now}";
            _fileStream = new(fileName, FileMode.CreateNew, FileAccess.Write);
            _streamWriter = new(_fileStream);
            _streamWriter.AutoFlush = true;
        }

        string str = line.ToString() ?? "";
        _streamWriter.WriteLine(str);
        _fileStream.Flush();
    }
}
#else
public static class Log
{
    public static void WriteLine(object _) { }
}
#endif
