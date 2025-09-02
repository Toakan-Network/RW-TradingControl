using RimWorld;
using UnityEngine;
using Verse;

namespace TradingControl.definitions
{
    [DefOf]
    public static class DismissPawn
    {
        public static JobDef DismissAny;

    }

    [DefOf]
    public static class TC_DefOf
    {
        public static ThingDef Marketplace;
        public static ThingDef TradingSpot;
        public static ThingDef DropSpotTradeShip;
    }
    [DefOf]
    public static class TraderBuff
    {
        public static ThingDef TC_TraderBuff;
    }

    [DefOf]
    public static class TC_HediffDefOf
    {
        public static HediffDef TC_TradeBuff;
    }
}
