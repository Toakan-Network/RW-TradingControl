﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RimWorld;
using Verse;


namespace TradingControl.Settings
{
    [StaticConstructorOnStartup]
    public class TradingControlSettings : ModSettings
    {
        public bool TradingSpotEnabled = TradingControl.init.Initialize.TradingSpotEnabled();
        public bool VisitorsGoToTradeSpot = true;
        public bool TradersGoToTradeSpot = true;
        public bool FlagMarker = true;
        public float MaxTradeSpot = 1;
        public bool punchThroughEnabled = false;

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Values.Look(ref TradingSpotEnabled, "TradingControl.TradingSpotEnabled", TradingSpotEnabled, true);
            Scribe_Values.Look(ref TradersGoToTradeSpot, "TradingControl.TradersGoToTradeSpot", TradersGoToTradeSpot, true);
            Scribe_Values.Look(ref VisitorsGoToTradeSpot, "TradingControl.VisitorsGotoTradeSpot", VisitorsGoToTradeSpot, true);
            Scribe_Values.Look(ref MaxTradeSpot, "TradingControl.MaxTradeSpot", MaxTradeSpot, true);
            //Scribe_Values.Look(ref punchThroughEnabled, "TradingControl.punchThroughEnabled", punchThroughEnabled, true);
            //Scribe_Values.Look<bool>(ref FlagMarker, "TradingControl.UseFlagMarker", false, false);
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

            GraphicData prevGraphicData = ThingDefOf.PartySpot.graphicData;
            GraphicData marker = new GraphicData
            {
                texPath = "Building/Misc/TradingControl/Flag"
            };
            marker.drawSize.Set(1, 1);
            marker.graphicClass = typeof(Graphic_Single);

            if (LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().FlagMarker)
            {
                ThingDefOf.PartySpot.graphicData = marker;
            }
            else
            {
                ThingDefOf.PartySpot.graphicData = prevGraphicData;
            }


            base.WriteSettings();
        }

        

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard l = new Listing_Standard();
            l.Begin(new Rect(0, 80, 300, 100));
            //l.CheckboxLabeled("TradingControl.TradingSpotEnabled".Translate(), ref TradingSpotEnabled);

            l.CheckboxLabeled("TradingControl.TradersGoToTradeSpot".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().TradersGoToTradeSpot);
            
            l.CheckboxLabeled("TradingControl.VisitorsGoToTradeSpot".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().VisitorsGoToTradeSpot);
            
            //l.CheckboxLabeled("TradingControl.punchThroughEnabled".Translate(), ref LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().punchThroughEnabled);

            l.Label("TradingControl.MaxTradeSpot".Translate() +((int) LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot) + ".");
            LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot = l.Slider(LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().MaxTradeSpot, 1f, 10f);
                        
            //l.CheckboxLabeled("TradingControl.UseFlagMarker".Translate(), ref FlagMarker);
            l.End();
        }
    }
}
