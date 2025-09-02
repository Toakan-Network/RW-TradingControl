using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using TradingControl.definitions;
using TradingControl.functions;
using TradingControl.Settings;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.AI.Group;

namespace TradingControl.functions
{
    public class SharedActions
    {
        private readonly List<Building> _spots = new List<Building>();

        public bool CanHandleTraderOrVisitor(Pawn targetPawn)
        {
            // Target Checks
            if (targetPawn == null)
                return false;

            if (targetPawn.lord?.LordJob == null)
                return false;

            // Null check for mindState
            if (targetPawn.mindState == null)
                return false;

            // Either Visitor or Trader
            if (CheckVisitor(targetPawn.lord.LordJob))
                return true;
                    
            if (CheckTrader(targetPawn.lord.LordJob))
                return true;
            
            return false;
        }

        public bool CanHandleColonist(Pawn colonist)
        {
            if (colonist == null)
                return false;

            if (!colonist.IsColonistPlayerControlled)
                return false;

            if (!colonist.RaceProps.Humanlike)
                return false;

            if (colonist.DeadOrDowned)
                return false;

            if (colonist.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
                return false;

            return true;
        }

        private bool CheckVisitor(Verse.AI.Group.LordJob lordJob)
        {
            if (lordJob is LordJob_VisitColony)
            {
                return true;
            }
            return false;
        }

        private bool CheckTrader(Verse.AI.Group.LordJob lordJob)
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
        
        public void CheckForPreviousSpot(Building building)
        {
            if (Current.Game?.CurrentMap == null)
                return;

            if (building == null)
                return;

            var maxTradeSpotSetting = ((int)LoadedModManager.GetMod<TradingControlMod>()
                .GetSettings<TradingControlSettings>().MaxTradeSpot);
            var RequiresWorkToRemove = (bool)LoadedModManager.GetMod<TradingControlMod>()
                .GetSettings<TradingControlSettings>().RequiresWorkToRemove;
            
            _spots.Clear();

            if (building.GetType() == typeof(TradingSpot) || building.GetType() == typeof(Marketplace))
                foreach (Building b in Current.Game.CurrentMap.listerBuildings.allBuildingsColonist.FindAll(x => x.GetType() == typeof(TradingSpot) || x.GetType() == typeof(Marketplace)))
                    _spots.Add(b);

            if (building.GetType() == typeof(DropSpotIndicator))
                foreach (Building b in Current.Game.CurrentMap.listerBuildings.allBuildingsColonist.FindAll(x => x.GetType() == typeof(DropSpotIndicator)))
                    _spots.Add(b);


            Designator marker = new Designator_Deconstruct();
            while (_spots.Count >= maxTradeSpotSetting)
            {
                Building oldest = _spots[0];
                Messages.Message("TradingControl.AlreadyOnMap".Translate(), MessageTypeDefOf.NeutralEvent, false);

                // Check if Building is already designated for deconstruction
                Designation marked = Current.Game.CurrentMap.designationManager.DesignationOn(oldest, DesignationDefOf.Deconstruct);
                if (marked == null && RequiresWorkToRemove)
                    marker.DesignateThing(oldest);

                if (!RequiresWorkToRemove)
                    oldest.Destroy();

                _spots.RemoveAt(0);
            }
        }
    }
}
