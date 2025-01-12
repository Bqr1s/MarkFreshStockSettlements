using System.Collections.Generic;
using System.Reflection;
using Verse;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;
using UnityEngine;

namespace MarkFreshStockSettlements
{
	[StaticConstructorOnStartup]
	public class Main
	{
		static Main()
		{
			Harmony pat = new Harmony("MarkFreshStockSettlements");
			pat.PatchAll();
		}
	}

	[HarmonyPatch(typeof(WorldSelectionDrawer), "SelectionOverlaysOnGUI")]
	public static class ShowRestockedSettlements
	{
		public static void Postfix(WorldObject __instance)
		{
			List<Settlement> settlements = Find.WorldObjects.Settlements;
			foreach (Settlement s in settlements)
			{
				if (s.Visitable && (!s.EverVisited || s.RestockedSinceLastVisit))
				{
					DrawHighlightStarOnGUIFor(s);
				}
			}
		}
		private static void DrawHighlightStarOnGUIFor(WorldObject obj)
		{
			if (obj.HiddenBehindTerrainNow())
				return;
			Vector2 vector = obj.ScreenPos();
			Vector2[] bracketLocations = new Vector2[4];
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(rect: new Rect(vector.x - 17.5f, vector.y - 17.5f, 35f, 35f),
				textureSize: new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * 0.4f, (float)SelectionDrawerUtility.SelectedTexGUI.height * 0.4f),
				bracketLocs: bracketLocations, obj: obj, selectTimes: WorldSelectionDrawer.SelectTimes, jumpDistanceFactor: 25f);

			// Scale and offset manually piced for one specific HighDPI workplace for now
			float starScale = 0.15f; // Find.WorldGrid.averageTileSize / star.width / 1.8f;
			float tileSize = GenWorldUI.CurUITileSize();
			// This works for when zoomed in, star offsets more away from the icon.
			// But when zoomed out and settlements icons are too close to each other, stars get closer to their settlement too.
			float offsetCoeeficient = Mathf.Min(tileSize / 10.0f, 1);
			Texture star = ContentFinder<Texture2D>.Get("Star2");
			bracketLocations[0].x -= star.width * starScale / 3.0f * offsetCoeeficient;
			bracketLocations[0].y -= star.height * starScale / 3.0f * offsetCoeeficient;
			Widgets.DrawTextureRotated(bracketLocations[0], star, 0, starScale);
		}
	}
}

