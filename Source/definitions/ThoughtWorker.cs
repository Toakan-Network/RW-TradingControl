using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingControl.Settings;
using Verse;

namespace TradingControl.definitions
{
    public class ThoughtWorker_TradeAura : ThoughtWorker
    {
        private static readonly ThingDef[] AuraBuildingDefs =
        {
            TC_DefOf.Marketplace,
            TC_DefOf.DropSpotTradeShip,
            TC_DefOf.TradingSpot
        };

        private enum AuraLevel
        {
            None = 0,
            Minimal = 1,
            Normal = 2,
            Overwhelming = 3
        }

        private const float MinAuraRadius = 16f;
        private const float NormalAuraRadius = 10f;
        private const float OverwhelmingAuraRadius = 5f;

        private static AuraLevel DetermineLevel(float distance)
        {
            if (distance <= OverwhelmingAuraRadius) return AuraLevel.Overwhelming;
            if (distance <= NormalAuraRadius) return AuraLevel.Normal;
            if (distance <= MinAuraRadius) return AuraLevel.Minimal;
            return AuraLevel.None;
        }

        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p == null || p.Map == null || p.Dead) return ThoughtState.Inactive;
            if (!(LoadedModManager.GetMod<TradingControlMod>().GetSettings<TradingControlSettings>().UseSocialMoodTradeBuff))
                return ThoughtState.Inactive;


            float nearestDist = MinAuraRadius;
            // Find nearest building of trade type
            for (int d = 0; d < AuraBuildingDefs.Length; d++)
            {
                List<Thing> things = p.Map.listerThings.ThingsOfDef(AuraBuildingDefs[d]);
                if (things == null || things.Count == 0) continue;

                for (int i = 0; i < things.Count; i++)
                {
                    Thing t = things[i];
                    float dist = p.Position.DistanceTo(t.Position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        if (nearestDist <= OverwhelmingAuraRadius)
                            break; // cannot get stronger than this
                    }
                }
            }

            if (nearestDist == MinAuraRadius)
                return ThoughtState.Inactive; // no sources on map

            AuraLevel level = DetermineLevel(nearestDist);
            if (level == AuraLevel.None)
                return ThoughtState.Inactive;

            // Stages must be ordered: Minimal (0), Normal (1), Overwhelming (2)
            int stageIndex = ((int)level) - 1;
            return ThoughtState.ActiveAtStage(stageIndex);
        }
    }
}
