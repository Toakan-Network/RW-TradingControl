
using UnityEngine;
using Verse;
using RimWorld;
using TradingControl;

namespace TradingControl.functions
{
    [StaticConstructorOnStartup]
    public static class OrbitDropSpot
    {
        public static bool AnyAdjacentGoodDropSpot(IntVec3 c, Map map, bool allowFogged, bool canRoofPunch)
        {
            return DropCellFinder.IsGoodDropSpot(c + IntVec3.North, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.East, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.South, map, allowFogged, canRoofPunch) || DropCellFinder.IsGoodDropSpot(c + IntVec3.West, map, allowFogged, canRoofPunch);
        }

        public static readonly Texture2D ColorWheel = ContentFinder<Texture2D>.Get("colorwheel", true);
    }
    public class DropSpotIndicator : Building
    {
        public DropSpotIndicator()
        {
            if (Current.Game.CurrentMap != null)
            {
                if (Current.Game.CurrentMap.listerBuildings.allBuildingsColonist.Find(x => x is DropSpotIndicator) is DropSpotIndicator dropSpot)
                {
                    dropSpot.Destroy();
                    Messages.Message("TradingControl.OrbitTraderAlreadyOnMap".Translate(), MessageTypeDefOf.NeutralEvent, false);
                }
            }
        }
    }

}
