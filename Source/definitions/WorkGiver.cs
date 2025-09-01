using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace TradingControl.definitions
{
    internal class WorkGiver_Dismiss : WorkGiver
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return false;
        }

        public IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) => null;
        public Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DismissPawn.DismissAny, t);
        }
    }
}
