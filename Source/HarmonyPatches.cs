using System;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using UnityEngine;
using Verse.AI;
using RimWorld;
using TradingControl.functions;
using TradingControl.functions;

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
                //LogHandler.LogInfo("#1 - Setting up Dismiss options.");
                // Need to override the CanDismisswithRightClick in ChoiceLetter classes.

                //instance.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders"), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(Dismisstraders)));

                LogHandler.LogInfo("#2 - Patching Toils (Job Manager)");
                instance.Patch(AccessTools.Method(typeof(JobDriver), "SetupToils"));

                LogHandler.LogInfo("#3 - Patching Orbital Drop Sites");
                instance.Patch(AccessTools.Method(typeof(DropCellFinder), nameof(DropCellFinder.TradeDropSpot)), prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CustomTradeDropSpot)));

                //LogHandler.LogInfo("#4 - Patching Oribtal Requests");
                //instance.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanLikeOrders"), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(OrbitalRequest)));
                
                LogHandler.LogInfo("## - Harmony Patches Applied");
            }
            catch (Exception ex)
            {
                LogHandler.LogError("Error found when Patching : ", ex);
            }
            LogHandler.LogInfo("## - No further Patches detected..");
        }
        // -------------- End of HarmonyPatches() ----------------- //
        // -------------- Starting to run around ----------------- //
        static void Postfix(JobDriver __instance)
        {
            if (!(__instance is JobDriver_Goto))
            {
                return;
            }
            JobDriver_Goto jobDriver = (JobDriver_Goto)__instance;
            List<Toil> toils = Traverse.Create(jobDriver).Field("toils").GetValue<List<Toil>>();
            if (toils.Count() > 0)
            {
                Toil toil = toils.ElementAt(0);
                toil.AddPreTickAction(delegate { });
            }
        }
        // -------------- Not running around anymore ----------------- //
        // -------------- Set up the Dismiss Traders options ----------------- //
        static void Dismisstraders(ref Vector3 clickPos, ref Pawn pawn, ref List<FloatMenuOption> opts)
        {
            foreach (LocalTargetInfo target in GenUI.TargetsAt(clickPos, TargetingParameters.ForAttackAny()))
            {
                Pawn colonist = pawn;
                LocalTargetInfo dest = target;
                Pawn playerTarget = (Pawn)dest.Thing;
                if (!TradingControl.functions.Control.CanDismiss(colonist, dest)) { return; }
                // This bit tells the Colonist to leave.
                void Action()
                {
                    try
                    {
                        functions.Control.Leave(colonist, dest, playerTarget);
                    }
                    catch (Exception ex)
                    {
                        LogHandler.LogError("Exception Caught: ", ex);
                    }
                }

                // Creates the value for the Context Menu.
                string ContextMenuValue = functions.Control.ContextMenu(playerTarget);

                // Creates the right click Context Menu.
                opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(ContextMenuValue, Action, MenuOptionPriority.InitiateSocial, null, dest.Thing), pawn, playerTarget,null, null));
            }
        }

        // -------------- Finish the Dismiss Traders options ----------------- //
        // ------------------ Setup Orbital Drop site -------------------//
        public static bool CustomTradeDropSpot(Map map, ref IntVec3 __result)
        {
            //bool punchThrough = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().punchThroughEnabled;
            bool punchThrough = false;

            DropSpotIndicator dropSpotIndicator = map.listerBuildings.allBuildingsColonist.Find(x => x is DropSpotIndicator) as DropSpotIndicator;
            if (dropSpotIndicator != null && !map.roofGrid.Roofed(dropSpotIndicator.Position) && OrbitDropSpot.AnyAdjacentGoodDropSpot(dropSpotIndicator.Position, map, false, punchThrough))
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
        // ------------------ Finish Orbital Drop site -------------------//

        // ------------------ Setup Orbital requests-------------------//


        // ------------------ Finish Orbital requests -------------------//
    }
}

