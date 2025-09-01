using RimWorld;
using System;
using System.Collections.Generic;
using TradingControl.functions;
using TradingControl.Harmonize;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static TradingControl.Harmonize.HarmonyPatches;

namespace TradingControl.Dismiss
{
    [StaticConstructorOnStartup]
    class Control
    {
        public static bool CanDismiss(Pawn pawn, LocalTargetInfo destination)
        {
            if (pawn == null || destination == null) return false;
            if (!pawn.CanReach(destination, PathEndMode.OnCell, Danger.Deadly)) return false;
            if (pawn.skills.GetSkill(SkillDefOf.Social).TotallyDisabled) return false;
            return true;
        } 

        public static bool HasFleeingDuty(Pawn pawn)
        {
            return pawn.mindState.duty != null && (pawn.mindState.duty.def == DutyDefOf.ExitMapRandom || pawn.mindState.duty.def == DutyDefOf.Steal || pawn.mindState.duty.def == DutyDefOf.Kidnap);
        }
        public static void Leave(Pawn playerTarget)
        {
            if ((playerTarget.trader.CanTradeNow || playerTarget.mindState.wantsToTradeWithColony))
            {
                Lord lord = playerTarget.GetLord();
                try
                {
                    CreateLeaveTransition(lord);
                }
                catch (Exception ex)
                {
                    LogHandler.LogError("Unable to create ForceLeave transition.", ex);
                }
                //lord.GotoToil(lord.Graph.lordToils[0]);
                Messages.Message("Caravan from " + lord.faction.Name.ToString() + " was dismissed", MessageTypeDefOf.NeutralEvent, false);
            }
        }
        public static void CreateLeaveTransition(Lord lord)
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

        

    }

    class Setup
    {
        // -------------- Set up the Dismiss Traders options ----------------- //
        public static void DismissTraders(ref Vector3 clickPos, ref Pawn pawn, ref List<FloatMenuOption> opts)
        {
            foreach (LocalTargetInfo target in GenUI.TargetsAt(clickPos, TargetingParameters.ForAttackAny()))
            {
                Pawn colonist = pawn;
                var dest = target;
                Pawn playerTarget = (Pawn)dest.Thing;
                if (!Control.CanDismiss(colonist, dest)) { return; }
                // This bit tells the Colonist to leave.
                //void Action()
                //{
                //    try
                //    {
                //        Control.Leave(playerTarget);
                //    }
                //    catch (Exception ex)
                //    {
                //        LogHandler.LogError("Exception Caught: ", ex);
                //    }
                //}

                // Creates the value for the Context Menu.
                //string ContextMenuValue = Control.ContextMenu(playerTarget);

                // Creates the right click Context Menu.
                //opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(ContextMenuValue, Action, MenuOptionPriority.InitiateSocial, null, dest.Thing), pawn, playerTarget, null, null));
            }
        }

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


    }
}
