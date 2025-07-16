using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI.Group;
using TradingControl.Settings;

namespace TradingControl.TradingSpot
{
    public class TradingSpot : Building
    {        
        public List<Building> spots = new List<Building>();
        private int count = 0;
        public TradingSpot()
        {
            if (Current.Game.CurrentMap != null)
            {
                int MaxTradeSpotSetting = ((int)LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot);
                int counter = 0;

                foreach (Building b in Current.Game.CurrentMap.listerBuildings.allBuildingsColonist)
                {
                    if (b.def.defName.Equals("TradingSpot"))
                    {
                        counter++;
                        spots.Add(b);
                        if (counter >= MaxTradeSpotSetting)
                        {
                            Building ds = spots[0];
                            Messages.Message("TradingControl.AlreadyOnMap".Translate(), MessageTypeDefOf.NeutralEvent, false);
                            ds.Destroy(DestroyMode.Vanish);
                            break;
                        }
                    }
                }
            }
        }

        protected override void Tick()
        {
            base.Tick();
            if (count++ > 1)
            {
                count = 0;
                List<Lord> lords = Map.lordManager.lords;
                IntVec3 position = Position;
                bool VistorSetting = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().VisitorsGoToTradeSpot;
                bool TraderSetting = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().TradersGoToTradeSpot;
                int MaxTradeSpotSetting = ((int) LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot);
                checked
                {
                    for (int i = 0; i < lords.Count; i++)
                    {
                        Lord lord = lords[i];
                        if ((CheckVisitor(lord.LordJob) && VistorSetting) || CheckTrader(lord.LordJob) && TraderSetting )
                        {
                            FieldInfo field = lord.LordJob.GetType().GetField("chillSpot", BindingFlags.Instance | BindingFlags.NonPublic);
                            IntVec3 intVec = (IntVec3)field.GetValue(lord.LordJob);
                            if (intVec.x != position.x || intVec.y != position.y || intVec.z != position.z)
                            {
                                field.SetValue(lord.LordJob, position);
                            }
                            LordToil curLordToil = lord.CurLordToil;
                            if (curLordToil is LordToil_Travel lordToil_Travel)
                            {
                                if (lordToil_Travel.FlagLoc != position)
                                {
                                    lordToil_Travel.SetDestination(position);
                                    lordToil_Travel.UpdateAllDuties();
                                }
                            }
                            else if (curLordToil is LordToil_DefendPoint lordToil_DefendPoint)
                            {
                                if (lordToil_DefendPoint.FlagLoc != position)
                                {
                                    lordToil_DefendPoint.SetDefendPoint(position);
                                    lordToil_DefendPoint.UpdateAllDuties();
                                }
                            }
                            foreach (Pawn current in lord.ownedPawns)
                            {
                                if (current.RaceProps.Animal)
                                {
                                    if (current.needs != null && current.needs.food != null && current.needs.food.CurLevel <= current.needs.food.MaxLevel)
                                    {
                                        current.needs.food.CurLevel = current.needs.food.MaxLevel;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

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
    }
}