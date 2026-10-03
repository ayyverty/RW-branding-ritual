using System.Collections.Generic;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// A selectable brand offered by the branding ritual. Each entry maps to a
	/// permanent hediff applied to the target once the player picks it.
	/// </summary>
	public class BrandDef : Def
	{
		/// <summary>Permanent hediff burned into the target's body.</summary>
		public HediffDef hediffDef;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in base.ConfigErrors())
			{
				yield return error;
			}

			if (hediffDef == null)
			{
				yield return $"{defName}: hediffDef is not set.";
			}
		}
	}
}