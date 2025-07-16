using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TradingControl.functions
{
    class OrbitalRequest
    {
        static class Patch_FloatMenuMakerMap_AddHumanlikeOrders
        {
            private static readonly List<TraderKindDef> orbitalTraders = new List<TraderKindDef>();
            private static readonly JobDef callTradeShip = DefDatabase<JobDef>.GetNamed("CallTradeShip", true);
            static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
            {
                foreach (Thing t in IntVec3.FromVector3(clickPos).GetThingList(pawn.Map))
                {
                    if (t is Building_CommsConsole)
                    {
                        addOptions(pawn, t, opts);
                        break;
                    }
                }
            }

            static void addOptions(Pawn pawn, Thing commsConsole, List<FloatMenuOption> opts)
            {
                if (orbitalTraders.Count == 0)
                {
                    foreach (var d in DefDatabase<TraderKindDef>.AllDefsListForReading)
                    {
                        if (d.orbital)
                            orbitalTraders.Add(d);
                    }
                    orbitalTraders.Sort(delegate (TraderKindDef d1, TraderKindDef d2)
                    {
                        return d1.label.CompareTo(d2.label);
                    });
                }

                if (!Util.HasEnoughSilver(commsConsole.Map, out int found))
                {
                    opts.Add(new FloatMenuOption("CallTradeShips.NotEnoughSilver".Translate(found, Settings.Cost), null));
                    return;
                }

                if (Settings.IsUsingTraderShipsMod())
                {
                    opts.Add(new FloatMenuOption(GetTraderShipsMenuLabel(), delegate ()
                    {
                        Job job = new Job_CallTradeShip(callTradeShip, commsConsole, null, TraderKindEnum.Lander);
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, true);
                    }, MenuOptionPriority.Low));

                    if (!Settings.AllowOrbitalTraders_ForTraderShipsMod)
                        return;
                }

                foreach (var d in orbitalTraders)
                {
                    opts.Add(new FloatMenuOption(GetMenuLabel(d), delegate ()
                    {
                        Job job = new Job_CallTradeShip(callTradeShip, commsConsole, d, TraderKindEnum.Orbital);
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, true);
                    }, MenuOptionPriority.Low));
                }
            }

            static string GetTraderShipsMenuLabel()
            {
                if (Settings.Cost > 0)
                    return "CallTradeShips.CallTraderShipCost".Translate(Settings.Cost);
                return "CallTradeShips.CallTraderShip".Translate();
            }

            static string GetMenuLabel(TraderKindDef d)
            {
                if (Settings.Cost > 0)
                    return "CallTradeShips.CallWithCost".Translate(d.LabelCap, Settings.Cost);
                return "CallTradeShips.Call".Translate(d.LabelCap);
            }
        }
    }
}