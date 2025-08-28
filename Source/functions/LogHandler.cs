using System;
using Verse;

namespace TradingControl.functions
{
    class LogHandler
    {
        private static string BuildInfo() 
        { 
            return $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} :: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}"; 
        }

        public static void LogError(string errorMessage)
        {
            Log.Error($"{BuildInfo()} - {errorMessage}");
        }
        public static void LogError(string errorMessage, Exception exception) 
        {
            Log.Error($"{BuildInfo()}  - {errorMessage} - {exception}");
        }

        public static void LogInfo(string infoMessage)
        {
           Log.Message($"{BuildInfo()}  - {infoMessage}");
        }

        public static void LogWarning(string warnMessage)
        {
            Log.Warning($"{BuildInfo()}  - {warnMessage}");
        }

        public static void LogDebug(string debugMessage, Exception exception = null)
        {
            if (DebugSettings.godMode)
            { Log.Message($"{BuildInfo()} :: Disable God Mode to suppress these errors."); }
            if(Prefs.DevMode || DebugSettings.godMode)
            {
                Log.Message($"{BuildInfo()} :: DEBUG :: {debugMessage}, {exception}");
            }
            
        }
    }
}
