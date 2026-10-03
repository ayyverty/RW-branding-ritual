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

			Log.Message($"[Branding Ritual] {target.LabelShortCap} branded with {brand.defName}.");
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