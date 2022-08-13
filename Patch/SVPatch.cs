using HarmonyLib;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class SVPatch {
		[HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
		[HarmonyPostfix]
		public static void SVCreateUIResPostFix() {
#if DEBUG
			Utils.DebugPrinter.PrintRecipes(SV.UIRes.RecipeData);
			Utils.DebugPrinter.PrintCraftCategories(SV.UIRes.CraftCategoryData);
#endif
			//This forces recipe to patch early lol
			if (SV.UIRes.RecipeData != null && SV.UIRes.CraftCategoryData != null) {
				return;
			}
			else {
				Plugin.log.LogError("Something wrong!");
			}
		}
	}
}
