using RimWorld;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// Opens the brand-selection dialog at the end of the branding stage, once
	/// the target has been delivered onto the spot and the brander has arrived.
	///
	/// Stage actions can fire more than once across a stage, so this guards
	/// against opening the dialog repeatedly or while the player is already in
	/// some other blocking dialog.
	/// </summary>
	public class RitualStageAction_ChooseBrand : RitualStageAction
	{
		public override void ExposeData()
		{
			// No fields to serialize.
		}

		public override void Apply(LordJob_Ritual ritual)
		{
			base.Apply(ritual);

			if (ritual == null || ritual.CurrentStage == null)
			{
				return;
			}

			// Only fire while the branding stage is active.
			if (ritual.StageIndex != BrandingConstants.BrandingStageIndex)
			{
				return;
			}

			if (Dialog_ChooseBrand.IsCurrentlyOpen)
			{
				return;
			}

			// Don't stack on top of another dialog the player needs to answer.
			if (Find.WindowStack.NonImmediateDialogWindowOpen)
			{
				return;
			}

			Pawn brander = ritual.PawnWithRole(BrandingRoleNames.Brander);
			Pawn target = ritual.PawnWithRole(BrandingRoleNames.Target);

			if (brander == null || target == null)
			{
				return;
			}

			Find.WindowStack.Add(new Dialog_ChooseBrand(brander, target));
		}
	}

	public static class BrandingConstants
	{
		/// <summary>Zero-based index of the branding stage in the behavior def.</summary>
		public const int BrandingStageIndex = 2;
	}
}