using HarmonyLib;
using RF5.RecipeMod.Recipe;
using System.Linq;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class CategoryPatch {
		public static bool categoryPatched = false;

		#region LOGGING
		private static void LogSizeBefore(CraftCategoryDataTable.CraftCategoryData[] craftData) {
			Plugin.log.LogInfo("Before patch:");
			foreach (var (categorySize, i) in craftData.Select((v, k) => (v.RecipeIds.Length, (CraftCategoryId)k))) {
				Plugin.log.LogInfo($"\tCategory {i} contains {categorySize} recipes");
			}
		}

		private static void LogSizeAfter(CraftCategoryDataTable.CraftCategoryData[] craftData) {
			Plugin.log.LogInfo("After patch:");
			foreach (var (categorySize, i) in craftData.Select((v, k) => (v.RecipeIds.Length, (CraftCategoryId)k))) {
				string logString = $"\tCategory {i} contains {categorySize} recipes";
				var newRecipeCount = RecipeLoader.Instance.newRecipeCategories[i].Count;
				if (newRecipeCount > 0) logString += $" ({newRecipeCount} custom recipes)";
				Plugin.log.LogInfo(logString);
			}
		}
		#endregion

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.CraftCategoryData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void AddCategoryRecipe(UIRes __instance, ref CraftCategoryDataTable __result) {
			if (categoryPatched) {
				return;
			}
			Plugin.log.LogInfo($"Patching category recipes");
			categoryPatched = true;

			var loadedRecipes = RecipeLoader.Instance;

			LogSizeBefore(__result.CraftCategoryDatas);

			foreach (var categoryList in loadedRecipes.newRecipeCategories) {
				__result.CraftCategoryDatas[(int)categoryList.Key].RecipeIds = __result.CraftCategoryDatas[(int)categoryList.Key].RecipeIds.Concat(categoryList.Value).ToArray();
			}

			LogSizeAfter(__result.CraftCategoryDatas);

			__instance._CraftCategoryDataTable = __result;
		}
	}
}
