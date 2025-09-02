using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using TradingControl.functions;


namespace TradingControl.definitions
{
    public class TC_TradeBuff_Hediff : Hediff
    {
        public int lastTickInRangeOfBuilding;
        private const float FallRatePerHour = 0.3f;
        private const float GainRatePerHour = 0.2f;
        private const int BufferTicks = 60;
        public bool InRangeOfBuilding => GenTicks.TicksGame <= lastTickInRangeOfBuilding + BufferTicks;

        public override bool ShouldRemove
        {
            get
            {
                if (!InRangeOfBuilding)
                    return base.ShouldRemove;
                
                return false;
            }
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            //pawn.health.RemoveHediff(this);
        }

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!InRangeOfBuilding)
            {
                // LogHandler.LogDebug("Reducing TradeBuff curValue: " + Severity);
                Severity -= 0.000132f * (float)delta;
            }
            else
            {
                // LogHandler.LogDebug("Increasing TradeBuff curValue: " + Severity);
                Severity += 6.4E-05f * (float)delta;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastTickInRangeOfBuilding, "lastTickInRangeOfBuilding", 0);
        }
    }
}   

