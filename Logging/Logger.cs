namespace TCGShopBetterPackOpener.Logging;

public class Logger
{
    public static void LogInfo(string message) => Plugin.Log.LogInfo(message);
    public static void LogWarning(string message) => Plugin.Log.LogWarning(message);
    public static void LogError(string message) => Plugin.Log.LogError(message);
    public static void LogDebug(string message) => Plugin.Log.LogDebug(message);
    public static void LogFatal(string message) => Plugin.Log.LogFatal(message);
}