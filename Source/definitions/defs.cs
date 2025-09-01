using RimWorld;
using UnityEngine;
using Verse;

namespace TradingControl.definitions
{
    [DefOf]
    public class Trading : BuildableDef
    {
        public string Name
        {
            get
            {
                return "TradingSpot".Translate();
            }
        }
            
        public string Label;
        public string ID;
        

        public GraphicData tradelocation;
        public string texturepath;
        public string graphic_type;

        public string altitude;

        public float flamability;
        public int worktobuild;
        
        public bool ScatterOnMapGen;
        public bool UseHP;

        public string Category;
        public string TickType;
            
    }

    [DefOf]
    public static class DismissPawn
    {
        public static JobDef DismissAny;

    }
}
