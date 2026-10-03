using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace SlaveBranding
{
	/// <summary>
	/// Makes the brander pick up the steel and hold it in both hands for the
	/// rest of the stage.
	///
	/// If the iron is not in the brander's inventory this declines to issue a
	/// job. That lets the duty fall through to JobGiver_TakeCountToInventory,
	/// which is declared ahead of this node and will fetch a replacement. This
	/// avoids an infinite re-issue loop when the iron is genuinely unavailable.
	/// </summary>
	public class JobGiver_HoldBrandingIron : ThinkNode_JobGiver
	{
		public const string HoldJobDefName = "toast_HoldBrandingIron";

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn == null || pawn.Drafted)
			{
				return null;
			}

			JobDef holdJob = DefDatabase<JobDef>.GetNamedSilentFail(HoldJobDefName);
			if (holdJob == null)
			{
				return null;
			}

			// Already carrying something: keep holding it wherever we are standing.
			Thing carried = pawn.carryTracker?.CarriedThing;
			if (carried != null && !carried.Destroyed)
			{
				return JobMaker.MakeJob(holdJob, carried);
			}

			Thing iron = FindIronInInventory(pawn);
			if (iron == null)
			{
				// Nothing to hold. Decline so an earlier node in the duty's think
				// tree (JobGiver_TakeCountToInventory) gets a chance to fetch one.
				return null;
			}

			return JobMaker.MakeJob(holdJob, iron);
		}

		private static Thing FindIronInInventory(Pawn pawn)
		{
			ThingOwner inventory = pawn.inventory?.GetDirectlyHeldThings();
			if (inventory == null)
			{
				return null;
			}

			for (int i = 0; i < inventory.Count; i++)
			{
				Thing thing = inventory[i];
				if (thing != null && !thing.Destroyed && thing.def == ThingDefOf.Steel)
				{
					return thing;
				}
			}

			return null;
		}
	}
}