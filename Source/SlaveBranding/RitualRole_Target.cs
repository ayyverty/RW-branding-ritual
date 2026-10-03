using RimWorld;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// The pawn being branded. Accepts any humanlike pawn regardless of faction
	/// or ideo, so the ritual works on your own colonists as well as on guests,
	/// prisoners and slaves.
	/// </summary>
	public class RitualRole_Target : RitualRole
	{
		public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn pawn = null, bool skipReason = false)
		{
			reason = null;
			return false;
		}

		public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null,
			RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
		{
			reason = null;

			if (p == null)
			{
				if (!skipReason)
				{
					reason = BrandingStrings.NoTarget;
				}
				return false;
			}

			if (!p.RaceProps.Humanlike || p.RaceProps.Animal)
			{
				if (!skipReason)
				{
					reason = BrandingStrings.NotHumanlike;
				}
				return false;
			}

			// The brander cannot brand themselves; the ritual needs two distinct pawns.
			if (ritual != null && ritual.PawnWithRole(BrandingRoleNames.Brander) == p)
			{
				if (!skipReason)
				{
					reason = BrandingStrings.BranderCantBrandSelf;
				}
				return false;
			}

			return true;
		}
	}

	public static class BrandingRoleNames
	{
		public const string Brander = "brander";
		public const string Target = "target";
	}
}