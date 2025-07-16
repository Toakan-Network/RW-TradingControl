using Verse;
using TradingControl.functions;

namespace TradingControl.init
{
    public static class Initialize
    {
        // Built for Mod Compatibility. When certain mods are detected, should it disable the Trading Spot?
        public static bool HospitalityCheck()
        {
            if (ModLister. HasActiveModWithName("Hospitality"))
            {
                LogHandler.LogInfo("ModName: Hospitality detected, turning Caravan Trading Spot off.");
                return true;
            }
            
            foreach (ModMetaData d in ModsConfig.ActiveModsInLoadOrder)
            {
                // Check for Hospitality
                if (d.PackageId.EqualsIgnoreCase("Orion.Hospitality"))
                {
                    return true;
                }
            }
            
            return false;
        }

        public static bool TradingSpotEnabled()
        {
            // Disable Trading spot if mod is found.
            if (HospitalityCheck()) 
            {
                return false;
            }
            // No mods found, keep Trading Spot enabled.
            return true;
        }

    }


}
