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
        public bool TradingSpotEnabled = TradingControl.init.Initialize.TradingSpotEnabled();
        public bool VisitorsGoToTradeSpot = true;
        public bool TradersGoToTradeSpot = true;
        public bool FlagMarker;
        public float MaxTradeSpot = 1;

        public bool MarketMarker;
        public bool punchThroughEnabled = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TradingSpotEnabled, "TradingControl.TradingSpotEnabled", TradingSpotEnabled, true);
            Scribe_Values.Look(ref TradersGoToTradeSpot, "TradingControl.TradersGoToTradeSpot", TradersGoToTradeSpot, true);
            Scribe_Values.Look(ref VisitorsGoToTradeSpot, "TradingControl.VisitorsGotoTradeSpot", VisitorsGoToTradeSpot, true);
            Scribe_Values.Look(ref MaxTradeSpot, "TradingControl.MaxTradeSpot", MaxTradeSpot, true);
            Scribe_Values.Look(ref punchThroughEnabled, "TradingControl.punchThroughEnabled", punchThroughEnabled, true);
            //Scribe_Values.Look(ref FlagMarker, "TradingControl.UseFlagMarker", false, true);
            //Scribe_Values.Look(ref MarketMarker, "TradingControl.MarketMarker", MarketMarker, true);
            base.ExposeData();
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
            Listing_Standard l = new Listing_Standard();
            l.Begin(new Rect(0, 80, 300, 300));
            l.CheckboxLabeled("TradingControl.TradingSpotEnabled".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().TradingSpotEnabled);

            l.CheckboxLabeled("TradingControl.TradersGoToTradeSpot".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().TradersGoToTradeSpot);
            
            l.CheckboxLabeled("TradingControl.VisitorsGoToTradeSpot".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().VisitorsGoToTradeSpot);
            
            l.CheckboxLabeled("TradingControl.punchThroughEnabled".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().punchThroughEnabled);

            l.Label("TradingControl.MaxTradeSpot".Translate() +((int) LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot) + ".");
            LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot = l.Slider(LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot, 1f, 10f);
                        
            // l.CheckboxLabeled( "TradingControl.UseFlagMarker".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().FlagMarker );
            // l.CheckboxLabeled( "Use the new Marketplace? (Beta)", ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MarketMarker);
            l.End();
        }
    }
}
