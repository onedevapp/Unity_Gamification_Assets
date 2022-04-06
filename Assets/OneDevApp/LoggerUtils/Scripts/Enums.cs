namespace SwipeWire
{
    public enum LogType : byte
    {
        Log = 0,
        Error = 1,
        Warning = 2
    }

    public enum LogLevel : byte
    {
        None = 0,
        Normal = 1,
        Verbose = 2
    }

    public enum LogOutput : byte
    {
        Unity = 0,
        Console = 1,
        FileOnly = 2
    }
}