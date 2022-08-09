using HarmonyLib;

namespace RF5.RecipeMod.Patches {
	[HarmonyPatch]
	internal class SVPatcher {
		[HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
		[HarmonyPostfix]
		public static void SVCreateUIResPatch() {
			RecipePatcher.recipePatched = false;
			CategoryPatcher.categoryPatched = false;

#if DEBUG
			Utils.DebugPrinter.PrintRecipes(SV.UIRes.RecipeData);
			Utils.DebugPrinter.PrintCraftCategories(SV.UIRes.CraftCategoryData);
#endif
		}
	}
}
