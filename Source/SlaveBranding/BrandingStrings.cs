using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// User-facing strings for the branding ritual.
	/// </summary>
	public static class BrandingStrings
	{
		public static string DialogTitle => "SB_BrandingDialogTitle".Translate();

		public static string DialogBody(Pawn target)
		{
			return "SB_BrandingDialogBody".Translate(target == null ? "" : target.LabelShortCap);
		}

		public static string ChooseLabel => "SB_BrandingChoose".Translate();
		public static string CancelLabel => "SB_BrandingCancel".Translate();
		public static string NoTarget => "SB_BrandingNoTarget".Translate();
		public static string NotHumanlike => "SB_BrandingNotHumanlike".Translate();
		public static string BranderCantBrandSelf => "SB_BrandingSelfTarget".Translate();
		public static string NoTargetLeft => "SB_BrandingNoTargetLeft".Translate();
	}
}