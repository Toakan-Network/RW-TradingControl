using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using TradingControl.definitions;

namespace TradingControl.functions
{
    public class FloatMenuOptionProvider_Dismiss : FloatMenuOptionProvider
    {
        private readonly SharedActions _sharedActions = new SharedActions();
        protected override bool Drafted => true;
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;

        // Simplest, robust dismissal: move the entire existing lord's pawn set to a new ExitMapBest lord.
        private static void Leave(Pawn targetPawn, Pawn actingColonist)
        {
            if (targetPawn == null || actingColonist == null) return;
            if (actingColonist.Faction == null || !actingColonist.Faction.IsPlayer) return;

            Job job = JobMaker.MakeJob(DismissPawn.DismissAny, targetPawn);
            job.playerForced = true;
            try
            {
                actingColonist.jobs.EndCurrentJob(JobCondition.InterruptForced, true, false);
                actingColonist.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
            catch(Exception ex)
            {
                LogHandler.LogError("Error giving colonist the job", ex);
            }
        }

        private static string ContextMenu(Pawn playerTarget)
        {
            string faction = playerTarget.Faction != null ? $"({playerTarget.Faction.Name})" : "(?)";
            string tradeName = playerTarget.Name?.ToStringFull ?? playerTarget.LabelShortCap ?? "Trader";
            return "TradingControl.Dismiss".Translate() + " " + tradeName + " " + faction;
        }

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Pawn targetPawn, FloatMenuContext context)
        {
            Pawn colonist = context.FirstSelectedPawn;
            if (!_sharedActions.CanHandleColonist(colonist))
                yield break;
            if (!_sharedActions.CanHandleTraderOrVisitor(targetPawn))
                yield break;
            if (targetPawn.GetLord() == null)
                yield break;
            if (!colonist.CanReach(targetPawn.Position, PathEndMode.OnCell, Danger.Deadly))
                yield break;
            if (colonist.skills?.GetSkill(SkillDefOf.Social)?.TotallyDisabled == true)
                yield break;

            string label = ContextMenu(targetPawn);
            yield return FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(
                    label,
                    () => Leave(targetPawn, context.FirstSelectedPawn),
                    MenuOptionPriority.InitiateSocial,
                    null,
                    null),
                colonist,
                targetPawn.Position,
                "ReservedBy: " + (colonist.Name ?? (Name)null),
                null);
        }
    }
}
