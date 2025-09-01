using RimWorld;
using System;
using System.Collections.Generic;
using TradingControl.Dismiss;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TradingControl.functions
{
    public class LordToil_ExitMapImmediate : LordToil
    {
        public LordToil_ExitMapImmediate() { }

        public override bool AllowSelfTend => true;
        public override void UpdateAllDuties()
        {
            if (this.lord?.ownedPawns == null)
                return;

            foreach (Pawn pawn in this.lord.ownedPawns)
            {
                if (pawn?.mindState == null)
                    continue;

                var traderCaravanRole = pawn.GetTraderCaravanRole();
                if (traderCaravanRole == TraderCaravanRole.Carrier || traderCaravanRole == TraderCaravanRole.Chattel)
                    pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBest)
                    {
                        locomotion = LocomotionUrgency.Jog
                    };
                else
                {
                    pawn.mindState.duty = new PawnDuty(DutyDefOf.ExitMapBestAndDefendSelf);
                }
            }
        }
    }

    public class LordJob_ExitImmediate : LordJob
    {
        public LocomotionUrgency locomotion;
        public bool CanDig = true;
        public bool CanInterruptJob = true;

        public LordJob_ExitImmediate(){}
        public LordJob_ExitImmediate(LocomotionUrgency locomotion, bool canDig = true, bool canInterruptJob = true)
        {
            this.locomotion = locomotion;
            this.CanDig = canDig;
            this.CanInterruptJob = canInterruptJob;
        }

        public override StateGraph CreateGraph()
        {
            LordToil leaveToil = new LordToil_ExitMapImmediate();

            StateGraph stateGraph = new StateGraph();
            LordToil exitMap = new LordToil_ExitMap(locomotion, CanDig, CanInterruptJob);
            exitMap.useAvoidGrid = true;
            
            stateGraph.AddToil(exitMap);
            stateGraph.AddToil(leaveToil);
            stateGraph.StartingToil = exitMap;
            
            var transitionForceLeave = new Transition(exitMap, leaveToil, true, true);
            transitionForceLeave.AddTrigger(new Trigger_TicksPassed(10));
            transitionForceLeave.AddPostAction(new TransitionAction_WakeAll());
            transitionForceLeave.AddPostAction(new TransitionAction_Message("TC | The caravan has been dismissed and is leaving the map.", MessageTypeDefOf.NeutralEvent));
            stateGraph.AddTransition(transitionForceLeave);

            return stateGraph;
        }


    }
}
