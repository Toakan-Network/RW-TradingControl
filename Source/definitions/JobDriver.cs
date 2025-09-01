using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingControl.Dismiss;
using TradingControl.functions;
using Unity.Jobs.LowLevel.Unsafe;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TradingControl.definitions
{
    internal class JobDriver_DismissAny : JobDriver
    {
        private Pawn DismissTarget => (Pawn)base.TargetThingA;
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOn(() => (DismissTarget.lord.LordJob.GetType() != typeof(LordJob_VisitColony) && DismissTarget.lord.LordJob.GetType()  != typeof(LordJob_TradeWithColony)));
            yield return new Toil
            {
                initAction = delegate
                {
                    DismissTarget.mindState.traderDismissed = true;
                    var caravanLord = DismissTarget.GetLord();
                    var curMap = caravanLord.Map;
                    caravanLord.Dispose();
                    caravanLord.Cleanup();
                    caravanLord.lordManager.RemoveLord(caravanLord);
                    LordMaker.MakeNewLord(DismissTarget.Faction, new LordJob_ExitMapBest(),curMap, null);

                    LogHandler.Notify("Caravan from " + DismissTarget.Faction.Name + " was dismissed.");
                    
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }

        public override bool TryMakePreToilReservations(bool onErrorFailed)
        {
            return pawn.Reserve(DismissTarget, job, 1, -1, null, onErrorFailed);
        }
    }
}
