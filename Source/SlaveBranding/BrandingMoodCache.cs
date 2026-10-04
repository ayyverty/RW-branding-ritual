using RimWorld;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// Cached result of the colony-wide branding check that the "all accounted
	/// for" and "unbranded" thoughts are built on.
	/// </summary>
	/// <remarks>
	/// Thought workers run once per pawn per mood recalculation. Scanning the
	/// colony from inside each worker would make a recalculation cost O(colonists
	/// squared), because every pawn would trigger a full scan of every pawn. This
	/// keeps the scan to at most one per RefreshTicks, shared by every pawn and
	/// every worker, and drops it early whenever a brand is applied.
	/// </remarks>
	public static class BrandingMoodCache
	{
		/// <summary>
		/// How long a scan stays valid. Long enough that an ordinary mood
		/// recalculation pass hits the cache, short enough that the only lag a
		/// player can notice is the brief window after a brand is applied, which
		/// ApplyBrand closes by calling Invalidate.
		/// </summary>
		private const int RefreshTicks = 250;

		private static int scannedAtTick = int.MinValue;
		private static int poolCount;
		private static int unbrandedCount;

		/// <summary>
		/// How many pawns in the pool are not branded, plus whether the pool is
		/// empty. An empty pool means there is nothing to be satisfied or
		/// dissatisfied about, and neither colony thought applies.
		/// </summary>
		public static void GetPoolState(out bool poolEmpty, out int unbranded)
		{
			int now = Find.TickManager?.TicksGame ?? 0;

			if (scannedAtTick == int.MinValue || now - scannedAtTick >= RefreshTicks)
			{
				Scan();
				scannedAtTick = now;
			}

			poolEmpty = poolCount == 0;
			unbranded = unbrandedCount;
		}

		/// <summary>
		/// Forces the next read to rescan. Called when a brand is applied so the
		/// mood thought reacts to the player's choice immediately.
		/// </summary>
		public static void Invalidate()
		{
			scannedAtTick = int.MinValue;
		}

		/// <summary>
		/// Walks the colony's pawns once, cheapest test first. The list is the
		/// cached PawnsFinder one, so this allocates nothing and also sees pawns
		/// away in caravans, not just those on a map. A pawn appears in it
		/// exactly once.
		/// </summary>
		private static void Scan()
		{
			poolCount = 0;
			unbrandedCount = 0;

			foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction)
			{
				if (pawn == null || !BrandingUtility.IsInBrandingPool(pawn))
				{
					continue;
				}

				poolCount++;

				if (!BrandingUtility.IsBranded(pawn))
				{
					unbrandedCount++;
				}
			}
		}
	}
}