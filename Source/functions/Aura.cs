using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingControl.definitions;
using Verse;

namespace TradingControl.functions
{

    public class Aura
    {
        private const float MaxAuraRadius = 28f;
        private const float NormalAuraRadius = 14f;
        private const float ClosestAuraRadius = 7f;

        public enum AuraType
        {
            None = 0,
            TradeBuff = 1
        }

        public static void AuraTick(Building building, AuraType aura)
        {
            //RemoveAura(building, aura);
            ApplyAura(building, AuraType.TradeBuff);
        }

        public static List<Pawn> NearbyPawns(Building building, float maxDistanceFromBuilding = MaxAuraRadius)
        {
            // Get list of pawns in within furthestDistance
            return building.Map.mapPawns.AllPawnsSpawned.Where(
                p => p.Position.DistanceTo(building.Position) <= maxDistanceFromBuilding
                     && p.IsColonist
            ).ToList();
        }

        private static bool CanApply(Pawn targetPawn)
        {
            if (targetPawn == null)
                return false;

            if (targetPawn.DeadOrDowned)
                return false;

            if (!targetPawn.IsColonist)
                return false;

            return true;
        }


        public static void ApplyAura(Building building, AuraType aura, float furthestDistance = MaxAuraRadius, float normalDistance = NormalAuraRadius, float closestDistance = ClosestAuraRadius)
        {
            if (building == null || building.Map == null)
                return;
            if (aura == AuraType.None)
                return;

            List<Pawn> affectedPawns = NearbyPawns(building, furthestDistance);

            // Check if we can apply Hediff to these pawns.
            foreach (Pawn p in affectedPawns)
            {
                if (CanApply(p))
                {
                    // LogHandler.LogDebug("Applying Aura Hediff.");
                    ((TC_TradeBuff_Hediff)p.health.GetOrAddHediff(TC_HediffDefOf.TC_TradeBuff)).lastTickInRangeOfBuilding = GenTicks.TicksGame;
                }
            }
        }
    }
}
