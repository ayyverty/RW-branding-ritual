using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace SlaveBranding
{
	/// <summary>
	/// Picks up the iron (reusing vanilla JobDriver_PickupToHold toils) and then
	/// holds it until the ritual stage ends. The job never completes on its own;
	/// it is replaced when the stage's duty changes or the ritual is cancelled.
	/// </summary>
	public class JobDriver_HoldBrandingIron : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return JobDriver_PickupToHold.TryMakePreToilReservations(this, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			foreach (Toil toil in JobDriver_PickupToHold.Toils(this, TargetIndex.A, true))
			{
				yield return toil;
			}

			// Hold position for the remainder of the stage. Ends only if the iron
			// is lost or the pawn can no longer hold it.
			Toil holdForever = new Toil
			{
				defaultCompleteMode = ToilCompleteMode.Never,
				handlingFacing = true,
				debugName = "toast_HoldBrandingIron"
			};

			holdForever.FailOn(() => pawn == null || pawn.carryTracker == null);
			holdForever.FailOn(() =>
			{
				Thing carried = pawn.carryTracker.CarriedThing;
				return carried == null || carried.Destroyed;
			});

			yield return holdForever;
		}
	}
}