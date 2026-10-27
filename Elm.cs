public static class Elm
{
    private static readonly object _lock = new();
    public enum HazardLevel
    {
        Debug,
        Log,
        Warning,
        Error,
        Unknown
    }
    public static class Config
    {
        public static string LoggingFilepath = "";
        public static HazardLevel MinimumReportableHazardLevel = HazardLevel.Debug;
        public static bool UseAsciiFallbacks = false;
        public static bool UseColoredOutput = true;
        public static string DebugTag = "🐛";
        public static string DebugTagAscii = "DBG";
        public static ConsoleColor DebugColor = ConsoleColor.Blue;
        public static string LogTag = "🌱";
        public static string LogTagAscii = "LOG";
        public static ConsoleColor LogColor = Console.ForegroundColor;
        public static string WarningTag = "🍂";
        public static string WarningTagAscii = "WRN";
        public static ConsoleColor WarningColor = ConsoleColor.Yellow;
        public static string ErrorTag = "🍄";
        public static string ErrorTagAscii = "ERR";
        public static ConsoleColor ErrorColor = ConsoleColor.Red;
        public static string UnknownTag = "🌳";
        public static string UnknownTagAscii = "???";
        public static ConsoleColor UnknownColor = ConsoleColor.Green;
    }
    public static event Action<string>? OnDebug;
    public static event Action<string>? OnLog;
    public static event Action<string>? OnWarning;
    public static event Action<string>? OnError;
    public static void OutputMessage(HazardLevel hazardLevel, string message)
    {
        lock (_lock)
        {
            if (Config.UseColoredOutput)
                Console.ForegroundColor = hazardLevel switch
                {
                    HazardLevel.Debug => Config.DebugColor,
                    HazardLevel.Log => Config.LogColor,
                    HazardLevel.Warning => Config.WarningColor,
                    HazardLevel.Error => Config.ErrorColor,
                    _ => Config.UnknownColor
                };
            string ResolvedTag = hazardLevel switch
            {
                HazardLevel.Debug => Config.UseAsciiFallbacks ? Config.DebugTagAscii : Config.DebugTag,
                HazardLevel.Log => Config.UseAsciiFallbacks ? Config.LogTagAscii : Config.LogTag,
                HazardLevel.Warning => Config.UseAsciiFallbacks ? Config.WarningTagAscii : Config.WarningTag,
                HazardLevel.Error => Config.UseAsciiFallbacks ? Config.ErrorTagAscii : Config.ErrorTag,
                _ => Config.UseAsciiFallbacks ? Config.UnknownTagAscii : Config.UnknownTag
            };
            string outString = $"[{DateTime.Now.ToString("HH:mm:ss")}][{ResolvedTag}]: {message}\n";
            if (!string.IsNullOrEmpty(Config.LoggingFilepath))
                File.AppendAllText(Config.LoggingFilepath, outString);
            Console.Write(outString);
            Console.ResetColor();
        }
    }
    public static void Debug(string message, bool attachStackTrace = false)
    {
        if (Config.MinimumReportableHazardLevel <= HazardLevel.Debug)
        {
            if (attachStackTrace)
                message = $"{message}\n\n{Environment.StackTrace}";
            OutputMessage(HazardLevel.Debug, message);
            OnDebug?.Invoke(message);
        }
    }
    public static void Log(string message, bool attachStackTrace = false)
    {
        if (Config.MinimumReportableHazardLevel <= HazardLevel.Log)
        {
            if (attachStackTrace)
                message = $"{message}\n\n{Environment.StackTrace}";
            OutputMessage(HazardLevel.Log, message);
            OnLog?.Invoke(message);
        }
    }
    public static void Warning(string message, bool attachStackTrace = false)
    {
        if (Config.MinimumReportableHazardLevel <= HazardLevel.Warning)
        {
            if (attachStackTrace)
                message = $"{message}\n\n{Environment.StackTrace}";
            OutputMessage(HazardLevel.Warning, message);
            OnWarning?.Invoke(message);
        }
    }
    public static void Error(string message, bool attachStackTrace = true)
    {
        if (Config.MinimumReportableHazardLevel <= HazardLevel.Error)
        {
            if (attachStackTrace)
                message = $"{message}\n\n{Environment.StackTrace}";
            OutputMessage(HazardLevel.Error, message);
            OnError?.Invoke(message);
        }
    }
    public static void BeginLoggingAt(string filepath)
    {
        string? directory = Path.GetDirectoryName(filepath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        Config.LoggingFilepath = filepath;
    }
    public static void BeginLoggingAtRelative(string filepath)
    {
        BeginLoggingAt(Path.Combine(Environment.CurrentDirectory, filepath));
    }
}
