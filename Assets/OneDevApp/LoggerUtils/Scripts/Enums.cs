namespace OneDevApp
{
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