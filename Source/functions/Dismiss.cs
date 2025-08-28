using RimWorld;
using System;
using System.Collections.Generic;
using TradingControl.functions;
using TradingControl.Harmonize;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static TradingControl.Harmonize.HarmonyPatches;

namespace TradingControl.functions
{
    [StaticConstructorOnStartup]
    class Control
    {

        /// Set Override to True for CanDismissWithRightClick for  class ChoiceLetter_AcceptVisitors : ChoiceLetter
        /// 

        public class ChoiceLetter_AcceptJoiner : ChoiceLetter
        {

            public override bool CanDismissWithRightClick => true; // Override to allow dismissing with right click.
            public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();

        }
        public class ChoiceLetter_BetrayVisitors : ChoiceLetter
        {
            public override bool CanDismissWithRightClick => true; // Override to allow dismissing with right click.
            public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();
        }

        public class ChoiceLetter_AcceptVisitors : ChoiceLetter
        {
            public override bool CanDismissWithRightClick => true; // Override to allow dismissing with right click.

            public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();
        }

        public class ChoiceLetter_AcceptCreepJoiner : ChoiceLetter
        {
            public override bool CanDismissWithRightClick => true; // Override to allow dismissing with right click.
            public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();
        }

        public class ChoiceLetter_OfferTrade : ChoiceLetter
        {
            public override bool CanDismissWithRightClick => true; // Override to allow dismissing with right click.
            public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();
        }


        public static bool CanDismiss(Pawn pawn, LocalTargetInfo destination)
        {
            if (!pawn.CanReach(destination, PathEndMode.OnCell, Danger.Deadly)) return false;
            if (pawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled) return false;
            return true;
        } 
        

        public static bool HasFleeingDuty(Pawn pawn)
        {
            return pawn.mindState.duty != null && (pawn.mindState.duty.def == DutyDefOf.ExitMapRandom || pawn.mindState.duty.def == DutyDefOf.Steal || pawn.mindState.duty.def == DutyDefOf.Kidnap);
        }
        public static void Leave(Pawn colonist, LocalTargetInfo target, Pawn playerTarget)
        {
            if ((target.Pawn.trader.CanTradeNow || target.Pawn.mindState.wantsToTradeWithColony))
            {
                Lord lord = playerTarget.GetLord();
                try
                {
                    CreateLeaveTransition(lord, colonist);
                }
                catch (Exception ex)
                {
                    LogHandler.LogError("Unable to create ForceLeave transition.", ex);
                }
                //lord.GotoToil(lord.Graph.lordToils[0]);
                Messages.Message("Caravan from " + lord.faction.Name.ToString() + " was dismissed", MessageTypeDefOf.NeutralEvent, false);
            }
        }
        public static void CreateLeaveTransition(Lord lord, Pawn colonist)
        {
            // Setup the Toil to leave map.
            LordToil LeaveMap = new LordToil_ExitMap(LocomotionUrgency.Jog, true, true);
            Transition transition_Forceleave = new Transition(lord.CurLordToil, LeaveMap, true, true);
            transition_Forceleave.AddTrigger(new Trigger_TicksPassed(1));
            transition_Forceleave.AddPostAction(new TransitionAction_WakeAll());
            lord.SetJob(new LordJob_ExitMapBest());
            
            lord.Graph.AddTransition(transition_Forceleave);
            lord.Graph.lordToils.Add(LeaveMap);
            lord.Graph.AddToil(LeaveMap);
            // lord.CurLordToil.UpdateAllDuties();

            lord.GotoToil(LeaveMap);
        }

        public static string ContextMenu(Pawn playerTarget)
        {
            // Sets up the Context Menu values.
            string faction = $"({playerTarget.Faction.Name})" ?? " (The wanderers... )";
            string tradeName = playerTarget.Name?.ToStringFull ?? playerTarget.Label ?? "Trader";
            return "TradingControl.Dismiss".Translate() + tradeName + faction; 
        }

    }
}
