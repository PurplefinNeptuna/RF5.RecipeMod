using HarmonyLib;
using RF5.RecipeMod.Recipe;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnityEngine;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class CategoryPatcher {
		//private static List<int> patchedSize = new();
		public static bool categoryPatched = false;

		//public static bool SameSize(List<int> origSize, List<int> patchSize) {
		//	if (origSize.Count != patchSize.Count) return false;
		//	for (int i = 0; i < origSize.Count; i++) {
		//		if (origSize[i] != patchSize[i]) return false;
		//	}
		//	return true;
		//}

		private static void LogSize(CraftCategoryDataTable.CraftCategoryData[] craftData, string status) {
			Plugin.log.LogInfo(status);
			//string logString = status;
			foreach (var (categorySize, i) in craftData.Select((v, k) => (v.RecipeIds.Length, (CraftCategoryId)k))) {
				//status += $"\n\tCategory {i} contains {categorySize} recipes";
				Plugin.log.LogInfo($"\tCategory {i} contains {categorySize} recipes");
			}
			//Plugin.log.LogInfo(logString);
		}

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.CraftCategoryData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void AddCategoryRecipe(UIRes __instance, ref CraftCategoryDataTable __result) {
			//if (SameSize(originalSize, patchedSize)) {
			if (categoryPatched) {
				return;
			}
			Plugin.log.LogInfo($"Patching Category Recipes");
			categoryPatched = true;

			var loadedRecipes = RecipeLoader.Instance;

			LogSize(__result.CraftCategoryDatas, "Before Patching:");

			foreach (var categoryList in loadedRecipes.newRecipeCategories) {
				__result.CraftCategoryDatas[(int)categoryList.Key].RecipeIds = __result.CraftCategoryDatas[(int)categoryList.Key].RecipeIds.Concat(categoryList.Value).ToArray();
			}

			LogSize(__result.CraftCategoryDatas, "After Patching:");

			__instance._CraftCategoryDataTable = __result;

			//patchedSize = new(newSize);
		}
	}
}
