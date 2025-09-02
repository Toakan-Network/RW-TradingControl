using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RimWorld;
using TradingControl.definitions;
using TradingControl.functions;
using Verse;


namespace TradingControl.Settings
{
    [StaticConstructorOnStartup]
    public class TradingControlSettings : ModSettings
    {
        public bool TradingSpotEnabled = TradingControl.Initialize.TradingSpotEnabled();
        public bool VisitorsGoToTradeSpot = true;
        public bool TradersGoToTradeSpot = true;
        public float MaxTradeSpot = 1;
        public bool PunchThroughEnabled = false;
        public bool RequiresWorkToPlace = true;
        public bool RequiresWorkToRemove = true;
        public int DefaultWorkValue = 500;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TradingSpotEnabled, "TradingControl.TradingSpotEnabled", TradingSpotEnabled, true);
            Scribe_Values.Look(ref TradersGoToTradeSpot, "TradingControl.TradersGoToTradeSpot", TradersGoToTradeSpot, true);
            Scribe_Values.Look(ref VisitorsGoToTradeSpot, "TradingControl.VisitorsGotoTradeSpot", VisitorsGoToTradeSpot, true);
            Scribe_Values.Look(ref MaxTradeSpot, "TradingControl.MaxTradeSpot", MaxTradeSpot, true);
            Scribe_Values.Look(ref PunchThroughEnabled, "TradingControl.punchThroughEnabled", PunchThroughEnabled, true);
            Scribe_Values.Look(ref RequiresWorkToPlace, "TradingControl.RequiresWorkToPlace", RequiresWorkToPlace, false);
            Scribe_Values.Look(ref RequiresWorkToRemove, "TradingControl.RequiresWorkToRemove", RequiresWorkToRemove, false);
            
            this.ApplySettings();
            base.ExposeData();
        }

        private void ApplySettings()
        {
            if (TC_DefOf.DropSpotTradeShip == null && TC_DefOf.Marketplace == null)
                return;

            var buildCostMarket = ThingDef.Named("Marketplace").statBases.Find(x => x.stat == StatDefOf.WorkToBuild);
            var buildCostOrbitalDrop = ThingDef.Named("DropSpotTradeShip").statBases.Find(x => x.stat == StatDefOf.WorkToBuild);
            if (!RequiresWorkToPlace)
            {
                buildCostMarket.value = 0;
                buildCostOrbitalDrop.value = 0;
            }
            else
            {
                buildCostMarket.value = DefaultWorkValue;
                buildCostOrbitalDrop.value = DefaultWorkValue;
            }
            
        }
    }

    public class TradingControlMod : Mod
    {

        public TradingControlMod(ModContentPack content) : base(content)
        {
             GetSettings<TradingControlSettings>();
        }

        public override string SettingsCategory()
        {
            return "Trading Control";
        }

        public override void WriteSettings()
        {

            base.WriteSettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var tradingControlModManager = LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>();

            Listing_Standard l = new Listing_Standard();
            l.Begin(new Rect(0, 80, 600, 600));
            l.Label("TradingControl.Settings.Description".Translate());
            l.CheckboxLabeled("TradingControl.TradingSpotEnabled".Translate(), ref tradingControlModManager.TradingSpotEnabled);
            l.Gap();
            l.Label("TradingControl.Settings.Behaviour".Translate());
            l.CheckboxLabeled("TradingControl.TradersGoToTradeSpot".Translate(), ref tradingControlModManager.TradersGoToTradeSpot);
            l.CheckboxLabeled("TradingControl.VisitorsGoToTradeSpot".Translate(), ref tradingControlModManager.VisitorsGoToTradeSpot);
            l.CheckboxLabeled("TradingControl.Settings.RequireWorkToPlace".Translate(), ref tradingControlModManager.RequiresWorkToPlace);
            l.CheckboxLabeled("TradingControl.Settings.RequireWorkToRemove".Translate(), ref tradingControlModManager.RequiresWorkToRemove);
            l.Label("TradingControl.MaxTradeSpot".Translate() + ((int)tradingControlModManager.MaxTradeSpot) + ".");
            l.Gap();
            l.Label("TradingControl.Settings.OrbitalBehaviour".Translate());
            l.CheckboxLabeled("TradingControl.punchThroughEnabled".Translate(), ref tradingControlModManager.PunchThroughEnabled);
            l.Gap();
            
            tradingControlModManager.MaxTradeSpot = l.Slider(tradingControlModManager.MaxTradeSpot, 1f, 10f);

            
            // l.CheckboxLabeled( "TradingControl.UseFlagMarker".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().FlagMarker );
            // l.CheckboxLabeled( "Use the new Marketplace? (Beta)", ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MarketMarker);
            l.End();
        }
    }
}
