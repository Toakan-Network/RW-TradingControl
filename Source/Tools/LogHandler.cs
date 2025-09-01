using RimWorld;
using System;
using Verse;

namespace TradingControl.functions
{
    class LogHandler
    {
        static string buildinfo() 
        { 
            return $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} :: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}"; 
        }

        public static void Notify(string infoMessage)
        {
            Messages.Message(infoMessage, MessageTypeDefOf.NeutralEvent, false);
        }

        public static void NotifyError(string infoMessage, Exception exception = null)
        {
            Messages.Message($"{infoMessage} - {exception}", MessageTypeDefOf.RejectInput, false);
            LogError(infoMessage, exception);
        }

        public static void LogError(string errorMessage)
        {
            Log.Error($"{buildinfo()} - {errorMessage}");
        }
        public static void LogError(string errorMessage, Exception exception) 
        {
            Log.Error($"{buildinfo()}  - {errorMessage} - {exception}");
        }

        public static void LogInfo(string infoMessage)
        {
           Log.Message($"{buildinfo()}  - {infoMessage}");
        }

        public static void LogWarning(string warnMessage)
        {
            Log.Warning($"{buildinfo()}  - {warnMessage}");
        }

        public static void LogDebug(string debugMessage, Exception exception = null)
        {
            if (DebugSettings.godMode)
            { Log.Message($"{buildinfo()} :: Disable Godmode to suppress these errors."); }
            if(Prefs.DevMode || DebugSettings.godMode)
            {
                Log.Message($"{buildinfo()} :: DEBUG :: {debugMessage}, {exception}");
            }
            
        }
    }
}
