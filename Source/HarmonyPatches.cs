using HarmonyLib;
using RimWorld;
using System;
using TradingControl.definitions;
using TradingControl.functions;
using TradingControl.Settings;
using Verse;

namespace TradingControl.Harmonize
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static Type patchType = typeof(HarmonyPatches);
        // -------------- Start of HarmonyPatches() ----------------- //
        static HarmonyPatches()
        {
            var instance = new Harmony("TradingControl.Tad.RW.Core");
            LogHandler.LogInfo("00 - Starting Harmony Patch...");
            try
            {
                LogHandler.LogInfo("01 - Patching Orbital Drop Sites");
                instance.Patch(AccessTools.Method(typeof(DropCellFinder), nameof(DropCellFinder.TradeDropSpot)), postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CustomTradeDropSpot)));

                //LogHandler.LogInfo("#4 - Patching Oribtal Requests");
                //instance.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanLikeOrders"), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(OrbitalRequest)));
                
                LogHandler.LogInfo("99 - Catching any last patches.");
                instance.PatchAll();

            }
            catch (Exception ex)
            {
                LogHandler.LogError("Error found when Patching : ", ex);
            }
            LogHandler.LogInfo("## - No further Patches detected..");
        }

        // ------------------ Setup Orbital Drop site -------------------//
        public static void CustomTradeDropSpot(Map map, ref IntVec3 __result)
        {
            if (map == null)
                return;

            bool punchThrough = LoadedModManager.GetMod<TradingControlMod>()
                .GetSettings<TradingControlSettings>().PunchThroughEnabled;

            // Get the Orbital Drop Spot Indicator(s)
            var dropSpotIndicators = map.listerBuildings.allBuildingsColonist
                .FindAll(x => x is DropSpotIndicator);
            if (dropSpotIndicators.Count == 0)
                return; // keep original result

            var indicator = dropSpotIndicators[0];

            if (OrbitDropSpot.AnyAdjacentGoodDropSpot(indicator.Position, map, false, punchThrough))
            {
                __result = indicator.Position;
                return;
            }

            LogHandler.LogInfo("No usable ground near Orbital Drop Site " + indicator + ". Using a clear, nearby area.");

            __result = CellFinderLoose.RandomCellWith(
                c => c.Standable(map) && !c.Fogged(map),
                map,
                1000);
        }
    }
}

