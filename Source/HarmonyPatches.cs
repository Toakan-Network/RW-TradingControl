using System;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;
using Verse.AI;
using RimWorld;
using TradingControl.functions;
using TradingControl.Settings;
using TradingControl.Dismiss;

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
            LogHandler.LogInfo("#0 - Starting Harmony Patch...");
            try
            {
                // 1.6 Lets us patch the ChoiceLetter classes to allow dismissing with right click.
                LogHandler.LogInfo("#1 - Setting up Dismiss options.");
                // Humanlike is no more :(
                // instance.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders"), null, new HarmonyMethod(typeof(Setup), nameof(Setup.DismissTraders)));

                LogHandler.LogInfo("#2 - Patching Toils (Job Manager)");
                instance.Patch(AccessTools.Method(typeof(JobDriver), "SetupToils"));

                LogHandler.LogInfo("#3 - Patching Orbital Drop Sites");
                instance.Patch(AccessTools.Method(typeof(DropCellFinder), nameof(DropCellFinder.TradeDropSpot)), prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CustomTradeDropSpot)));

                //LogHandler.LogInfo("#4 - Patching Oribtal Requests");
                //instance.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanLikeOrders"), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(OrbitalRequest)));
                
                instance.PatchAll();
                LogHandler.LogInfo("## - Harmony Patches Applied");
            }
            catch (Exception ex)
            {
                LogHandler.LogError("Error found when Patching : ", ex);
            }
            LogHandler.LogInfo("## - No further Patches detected..");
        }

        // ------------------ Setup Orbital Drop site -------------------//
        public static bool CustomTradeDropSpot(Map map, ref IntVec3 __result)
        {
            //bool punchThrough = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().punchThroughEnabled;
            bool punchThrough = false;

            if (map.listerBuildings.allBuildingsColonist.Find(x => x is DropSpotIndicator) is DropSpotIndicator dropSpotIndicator && !map.roofGrid.Roofed(dropSpotIndicator.Position) && OrbitDropSpot.AnyAdjacentGoodDropSpot(dropSpotIndicator.Position, map, false, punchThrough))
            {
                IntVec3 dropSpot = dropSpotIndicator.Position;
                if (!DropCellFinder.TryFindDropSpotNear(dropSpot, map, out IntVec3 singleDropSpot, false, false))
                {
                    LogHandler.LogInfo("No usable ground near Orbital Drop Site" + dropSpot + ". Using a clear, nearby area.");
                    singleDropSpot = CellFinderLoose.RandomCellWith((IntVec3 c) => c.Standable(map) && !c.Fogged(map), map, 1000);
                }
                __result = singleDropSpot;
                return false;
            }
            return true;
        }
    }
}

