using RimWorld;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// Shared audience test for the two colony-wide branding thoughts. Both look
	/// at the same cached pool state, so only one of them can ever trigger a
	/// scan.
	/// </summary>
	public abstract class BrandingThoughtWorker : ThoughtWorker
	{
		/// <summary>
		/// True for the pawns that should feel the colony-wide thoughts: the ones
		/// the player controls who are not themselves enslaved or imprisoned.
		/// Tested before the pool is touched, so guests, visitors and enslaved
		/// pawns never cause a scan.
		/// </summary>
		protected static bool ShouldFeelColonyThought(Pawn pawn)
		{
			return pawn != null
				&& pawn.IsColonistPlayerControlled
				&& !pawn.IsSlave
				&& !pawn.IsPrisoner;
		}
	}

	/// <summary>
	/// Positive thought: every slave, prisoner and non-supremacist-gender pawn
	/// the colony holds carries a brand.
	/// </summary>
	public class ThoughtWorker_AllBranded : BrandingThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!ShouldFeelColonyThought(p))
			{
				return ThoughtState.Inactive;
			}

			BrandingMoodCache.GetPoolState(out bool poolEmpty, out int unbranded);

			return !poolEmpty && unbranded == 0
				? ThoughtState.ActiveDefault
				: ThoughtState.Inactive;
		}
	}

	/// <summary>
	/// Negative thought: at least one of them is still unmarked.
	/// </summary>
	public class ThoughtWorker_NotAllBranded : BrandingThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (!ShouldFeelColonyThought(p))
			{
				return ThoughtState.Inactive;
			}

			BrandingMoodCache.GetPoolState(out bool poolEmpty, out int unbranded);

			return !poolEmpty && unbranded > 0
				? ThoughtState.ActiveDefault
				: ThoughtState.Inactive;
		}
	}

	/// <summary>
	/// Negative thought on a pawn who carries a brand themselves. Applies
	/// regardless of faction or standing, so a branded guest feels it too.
	/// </summary>
	public class ThoughtWorker_Branded : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return BrandingUtility.IsBranded(p)
				? ThoughtState.ActiveDefault
				: ThoughtState.Inactive;
		}
	}
}