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
                instance.Patch(AccessTools.Method(typeof(DropCellFinder), nameof(DropCellFinder.TradeDropSpot)), postfix: new HarmonyMethod(patchType, nameof(CustomTradeDropSpot)));

                LogHandler.LogInfo("02 - Patching Ship Landing Beacon Compatibility");
                var shipLandingUtil = AccessTools.TypeByName("RimWorld.ShipLandingBeaconUtility");
                if (shipLandingUtil != null)
                {
                    var landingZoneMethod = AccessTools.Method(shipLandingUtil, "LandingZoneCanBeUsed");
                    if (landingZoneMethod != null)
                    {
                        instance.Patch(landingZoneMethod, postfix: new HarmonyMethod(patchType, nameof(ShipLandingZoneFix)));
                        LogHandler.LogInfo("02 - Ship Landing patch applied.");
                    }
                    else
                    {
                        LogHandler.LogInfo("02 - LandingZoneCanBeUsed not found, skipping.");
                    }
                }
                else
                {
                    LogHandler.LogInfo("02 - Royalty not detected, skipping Ship Landing patch.");
                }

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

            // Indicator is not directly usable (e.g. enclosed by an animal pen or roofed area).
            // Try to find the nearest valid drop cell within a reasonable radius of the indicator.
            if (CellFinder.TryFindRandomCellNear(indicator.Position, map, 25,
                c => DropCellFinder.IsGoodDropSpot(c, map, false, punchThrough),
                out IntVec3 nearResult))
            {
                LogHandler.LogInfo("Orbital Drop Site " + indicator + " blocked; dropping near indicator.");
                __result = nearResult;
                return;
            }

            LogHandler.LogInfo("No usable ground near Orbital Drop Site " + indicator + ". Dropping to a random open area — consider relocating the indicator.");

            __result = CellFinderLoose.RandomCellWith(
                c => c.Standable(map) && !c.Fogged(map),
                map,
                1000);
        }

        // ------------------ Ship Landing Beacon Compatibility -------------------//
        // Postfix for ShipLandingBeaconUtility.LandingZoneCanBeUsed (Royalty DLC).
        // Prevents our DropSpotIndicator from being treated as a landing obstruction.
        public static void ShipLandingZoneFix(IntVec3 c, Map map, ref bool __result)
        {
            if (__result || map == null)
                return;

            bool hasOurDropSpot = false;
            bool hasOtherBlocker = false;

            foreach (Thing t in c.GetThingList(map))
            {
                if (t is DropSpotIndicator)
                {
                    hasOurDropSpot = true;
                }
                else if (t is Building)
                {
                    hasOtherBlocker = true;
                    break;
                }
            }

            if (hasOurDropSpot && !hasOtherBlocker)
                __result = true;
        }
    }
}

