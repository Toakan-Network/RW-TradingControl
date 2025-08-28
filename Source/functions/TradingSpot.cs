using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using TradingControl.functions;
using TradingControl.Settings;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.AI.Group;

namespace TradingControl.functions
{
    public class SharedActions
    {
        private List<Building> spots = new List<Building>();

        private bool CheckVisitor(LordJob lordJob)
        {
            if (lordJob is LordJob_VisitColony)
            {
                return true;
            }

            return false;
        }

        private bool CheckTrader(LordJob lordJob)
        {
            if (lordJob is LordJob_TradeWithColony)
            {
                return true;
            }

            return false;
        }

        public void GoToTradeSpot(IntVec3 buildingPosition, List<Lord> lordList)
        {
            var VistorSetting = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>()
                .VisitorsGoToTradeSpot;
            var TraderSetting = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>()
                .TradersGoToTradeSpot;
            var MaxTradeSpotSetting = ((int)LoadedModManager.GetMod<TradingControlMod>()
                .GetSettings<TradingControlSettings>().MaxTradeSpot);

            // For each Lord, check if they are a visitor or trader, then change their chill spot to the building position.
            for (int i = 0; i < lordList.Count; i++)
            {
                Lord lord = lordList[i];
                if ((CheckVisitor(lord.LordJob) && VistorSetting) || CheckTrader(lord.LordJob) && TraderSetting)
                {
                    FieldInfo field = lord.LordJob.GetType()
                        .GetField("chillSpot", BindingFlags.Instance | BindingFlags.NonPublic);
                    IntVec3 intVec = (IntVec3)field.GetValue(lord.LordJob);
                    if (intVec.x != buildingPosition.x || intVec.y != buildingPosition.y ||
                        intVec.z != buildingPosition.z)
                    {
                        field.SetValue(lord.LordJob, buildingPosition);
                    }

                    LordToil curLordToil = lord.CurLordToil;
                    if (curLordToil is LordToil_Travel lordToil_Travel)
                    {
                        if (lordToil_Travel.FlagLoc != buildingPosition)
                        {
                            lordToil_Travel.SetDestination(buildingPosition);
                            lordToil_Travel.UpdateAllDuties();
                        }
                    }
                    else if (curLordToil is LordToil_DefendPoint lordToil_DefendPoint)
                    {
                        if (lordToil_DefendPoint.FlagLoc != buildingPosition)
                        {
                            lordToil_DefendPoint.SetDefendPoint(buildingPosition);
                            lordToil_DefendPoint.UpdateAllDuties();
                        }
                    }

                    foreach (Pawn current in lord.ownedPawns)
                    {
                        if (current.RaceProps.Animal)
                        {
                            if (current.needs != null && current.needs.food != null &&
                                current.needs.food.CurLevel <= current.needs.food.MaxLevel)
                            {
                                current.needs.food.CurLevel = current.needs.food.MaxLevel;
                            }
                        }
                    }
                }
            }
        }

        public void CheckForPreviousSpot()
        {
            if (Current.Game?.CurrentMap == null)
                return;

            var maxTradeSpotSetting = ((int)LoadedModManager.GetMod<TradingControlMod>()
                .GetSettings<TradingControlSettings>().MaxTradeSpot);
            spots.Clear();

            foreach (Building b in Current.Game.CurrentMap.listerBuildings.allBuildingsColonist)
            {
                if (b.def.defName == "TradingSpot" || b.def.defName == "Marketplace")
                {
                    spots.Add(b);
                }
            }

            while (spots.Count >= maxTradeSpotSetting)
            {
                Building oldest = spots[0];
                Messages.Message("TradingControl.AlreadyOnMap".Translate(), MessageTypeDefOf.NeutralEvent, false);
                Designator marker = new Designator_Deconstruct();
                marker.DesignateThing(oldest);

                spots.RemoveAt(0);
            }
        }
    }


    public class MarketPlace : Building
    {
        public SharedActions Actions = new SharedActions();
        private int _count = 0;

        public MarketPlace()
        {
            Actions.CheckForPreviousSpot();
        }

        protected override void Tick()
        {
            base.Tick();
            if (_count++ > 1)
            {
                _count = 0;
                List<Lord> lords = Map.lordManager.lords;
                Actions.GoToTradeSpot(Position, lords);
            }
        }
    }


    public class TradingSpot : Building
    {
        public SharedActions Actions = new SharedActions();

        private int count = 0;

        public TradingSpot()
        {
            Actions.CheckForPreviousSpot();
        }

        protected override void Tick()
        {
            base.Tick();
            if (count++ > 1)
            {
                count = 0;
                List<Lord> lords = Map.lordManager.lords;
                Actions.GoToTradeSpot(Position, lords);
            }
        }
    }
}
