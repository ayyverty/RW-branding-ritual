using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SlaveBranding
{
	/// <summary>
	/// Lets the player choose which brand to burn into the target. Opened by
	/// RitualStageAction_ChooseBrand once the brandee is on the spot and the
	/// brander has arrived. Closing without choosing leaves the pawn unbranded.
	/// </summary>
	public class Dialog_ChooseBrand : Window
	{
		private const float RowHeight = 96f;
		private const float RowGap = 6f;

		private static readonly Color RowFill = new Color(0.16f, 0.15f, 0.14f, 0.55f);
		private static readonly Color RowOutline = new Color(0.32f, 0.30f, 0.28f, 1f);

		private readonly Pawn brander;
		private readonly Pawn target;
		private readonly List<BrandDef> brands;

		private Vector2 scrollPosition;

		/// <summary>Guards against applying the brand more than once.</summary>
		private bool resolved;

		public Dialog_ChooseBrand(Pawn brander, Pawn target)
		{
			this.brander = brander;
			this.target = target;

			brands = DefDatabase<BrandDef>.AllDefsListForReading
				.Where(brand => brand.hediffDef != null)
				.OrderBy(brand => brand.defName)
				.ToList();

			doCloseX = true;
			closeOnCancel = true;
			closeOnClickedOutside = false;
			optionalTitle = BrandingStrings.DialogTitle;
			absorbInputAroundWindow = true;
		}

		public override Vector2 InitialSize => new Vector2(500f, 560f);

		public static bool IsCurrentlyOpen => Find.WindowStack?.IsOpen(typeof(Dialog_ChooseBrand)) ?? false;

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.WordWrap = false;
			Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 24f), BrandingStrings.DialogTitle);

			Text.Font = GameFont.Small;
			Text.WordWrap = true;
			Widgets.Label(new Rect(inRect.x, inRect.y + 26f, inRect.width, 46f),
				BrandingStrings.DialogBody(target));
			Text.WordWrap = false;

			float listTop = inRect.y + 78f;
			const float footerHeight = 36f;
			Rect listRect = new Rect(inRect.x, listTop, inRect.width, inRect.height - listTop - footerHeight);

			float contentHeight = Math.Max(brands.Count * (RowHeight + RowGap), listRect.height);
			Rect viewRect = new Rect(0f, 0f, listRect.width - 24f, contentHeight);

			Widgets.BeginScrollView(listRect, ref scrollPosition, viewRect);

			float y = 0f;
			foreach (BrandDef brand in brands)
			{
				DrawBrandRow(brand, new Rect(0f, y, viewRect.width, RowHeight));
				y += RowHeight + RowGap;
			}

			Widgets.EndScrollView();

			if (Widgets.ButtonText(
					new Rect(inRect.x, inRect.height - footerHeight, 220f, 30f),
					BrandingStrings.CancelLabel))
			{
				resolved = true;
				Close();
			}

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private void DrawBrandRow(BrandDef brand, Rect rect)
		{
			Widgets.DrawBoxSolidWithOutline(rect, RowFill, RowOutline, 1);

			Rect inner = rect.ContractedBy(8f);
			Rect buttonRect = new Rect(inner.x, inner.yMax - 28f, 150f, 28f);

			Text.Font = GameFont.Medium;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.WordWrap = false;
			Widgets.Label(new Rect(inner.x, inner.y, inner.width, 22f), brand.label);

			Text.Font = GameFont.Tiny;
			Text.WordWrap = true;
			Widgets.Label(
				new Rect(inner.x, inner.y + 24f, inner.width, buttonRect.y - inner.y - 28f),
				brand.description);
			Text.WordWrap = false;

			Text.Font = GameFont.Small;
			if (Widgets.ButtonText(buttonRect, BrandingStrings.ChooseLabel))
			{
				ChooseBrand(brand);
			}

			Text.Anchor = TextAnchor.UpperLeft;
		}

		private void ChooseBrand(BrandDef brand)
		{
			if (resolved)
			{
				return;
			}
			resolved = true;

			BrandingUtility.ApplyBrand(brander, target, brand);
			Close();
		}

		public override bool OnCloseRequest()
		{
			// Closing via X or escape leaves the brandee unbranded, matching Cancel.
			resolved = true;
			return true;
		}
	}
}