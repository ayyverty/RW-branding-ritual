using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// Shared logic for applying a brand. Split out of the dialog so it can also
	/// be driven from tests or other call sites.
	/// </summary>
	public static class BrandingUtility
	{
		/// <summary>
		/// The hediff defs that count as a brand. Built once from the loaded
		/// BrandDefs and never rebuilt: DefDatabase is fully populated long before
		/// any pawn runs a thought worker, so rebuilding this per call would only
		/// burn time.
		/// </summary>
		private static HashSet<HediffDef> brandHediffs;

		/// <summary>Hediff defs that count as a brand.</summary>
		public static HashSet<HediffDef> BrandHediffs
		{
			get
			{
				if (brandHediffs == null)
				{
					brandHediffs = new HashSet<HediffDef>();

					List<BrandDef> brands = DefDatabase<BrandDef>.AllDefsListForReading;
					for (int i = 0; i < brands.Count; i++)
					{
						if (brands[i].hediffDef != null)
						{
							brandHediffs.Add(brands[i].hediffDef);
						}
					}
				}

				return brandHediffs;
			}
		}

		/// <summary>
		/// Consumes one piece of steel from the brander and burns the chosen brand
		/// into the target. The iron is the ritual's cost, so it is spent whether
		/// or not the brander was still visibly holding it.
		/// </summary>
		public static void ApplyBrand(Pawn brander, Pawn target, BrandDef brand)
		{
			if (brand == null || brand.hediffDef == null || target?.health == null)
			{
				return;
			}

			ConsumeIron(brander);

			Hediff hediff = target.health.GetOrAddHediff(brand.hediffDef);
			if (hediff != null)
			{
				hediff.Severity = brand.hediffDef.initialSeverity;
			}

			// The colony-wide mood thoughts are cached, so a new brand would not be
			// noticed until the cache window expired. Drop it now instead, so the
			// thought updates the moment the player closes the dialog.
			BrandingMoodCache.Invalidate();

			Log.Message($"[Branding Ritual] {target.LabelShortCap} branded with {brand.defName}.");
		}

		/// <summary>
		/// True if the pawn carries any of this mod's brands. Walks the pawn's
		/// hediff list once and probes a hash set per hediff, rather than asking
		/// about each brand in turn, which would rescan the list for every brand.
		/// </summary>
		public static bool IsBranded(Pawn pawn)
		{
			if (pawn?.health == null)
			{
				return false;
			}

			HashSet<HediffDef> brands = BrandHediffs;
			if (brands.Count == 0)
			{
				return false;
			}

			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i] != null && brands.Contains(hediffs[i].def))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// True when the pawn's gender is not the supreme gender of the ideology
		/// they follow. Pawns with no ideology name no supreme gender, so they are
		/// not counted either way.
		/// </summary>
		public static bool IsNonSupremacistGender(Pawn pawn)
		{
			Ideo ideo = pawn?.Ideo;
			if (ideo == null)
			{
				return false;
			}

			return pawn.gender != ideo.SupremeGender;
		}

		/// <summary>
		/// True for the pawns the colony-wide branding thoughts are measured
		/// against: its slaves, its prisoners, and its free colonists who are of a
		/// non-supremacist gender. Everyone else is outside the measurement.
		/// </summary>
		public static bool IsInBrandingPool(Pawn pawn)
		{
			if (pawn == null || pawn.Dead || !pawn.RaceProps.Humanlike)
			{
				return false;
			}

			// Slaves and prisoners are in the pool whatever their gender, so this
			// short-circuits before the more expensive ideology lookup below.
			if (pawn.IsSlaveOfColony || pawn.IsPrisonerOfColony)
			{
				return true;
			}

			if (!pawn.IsColonistPlayerControlled)
			{
				return false;
			}

			return IsNonSupremacistGender(pawn);
		}

		/// <summary>
		/// Removes one steel from the brander, taking it from what they are
		/// carrying first. Deliberately best-effort: if the iron is already gone
		/// the brand still applies rather than silently doing nothing.
		/// </summary>
		private static void ConsumeIron(Pawn brander)
		{
			if (brander == null)
			{
				return;
			}

			Thing carried = brander.carryTracker?.CarriedThing;
			if (carried != null && !carried.Destroyed && carried.def == ThingDefOf.Steel)
			{
				ConsumeFrom(carried);
				return;
			}

			ThingOwner inventory = brander.inventory?.GetDirectlyHeldThings();
			if (inventory == null)
			{
				return;
			}

			for (int i = 0; i < inventory.Count; i++)
			{
				Thing thing = inventory[i];
				if (thing != null && !thing.Destroyed && thing.def == ThingDefOf.Steel)
				{
					ConsumeFrom(thing);
					return;
				}
			}
		}

		private static void ConsumeFrom(Thing iron)
		{
			iron.stackCount -= 1;
			if (iron.stackCount <= 0)
			{
				iron.Destroy(DestroyMode.Vanish);
			}
		}
	}
}