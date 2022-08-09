using HarmonyLib;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class SVPatcher {
		[HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
		[HarmonyPrefix]
		public static void SVCreateUIResPatch() {
		}

		[HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
		[HarmonyPostfix]
		public static void SVCreateUIResPostFix() {
#if DEBUG
			Utils.DebugPrinter.PrintRecipes(SV.UIRes.RecipeData);
			Utils.DebugPrinter.PrintCraftCategories(SV.UIRes.CraftCategoryData);
#endif
		}
	}
}
