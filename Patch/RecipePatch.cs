using HarmonyLib;
using RF5.RecipeMod.Recipe;
using System.Linq;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class RecipePatch {
		private static int patchedSize = 0;
		public static bool recipePatched = false;

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.RecipeData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void RecipeDataPatch(UIRes __instance, ref RecipeDataTableArray __result) {
			AddRecipe(__instance, ref __result);
			CraftManagerPatch.FarmToSkillPatch();
		}

		private static void AddRecipe(UIRes uiRes, ref RecipeDataTableArray recipeTable) {
			var originalSize = recipeTable.RecipeDatas.Length;
			if (originalSize == patchedSize) {
				return;
			}

			if (recipePatched) {
				Plugin.log.LogError($"Recipe already patched\nLength: {originalSize}\nShould be: {patchedSize}");
			}

			Plugin.log.LogInfo($"Patching recipes");
			recipePatched = true;

			Plugin.log.LogInfo($"Before patch total {originalSize} recipes");

			recipeTable.RecipeDatas = recipeTable.RecipeDatas.Concat(RecipeLoader.Instance.newRecipes).ToArray();

			Plugin.log.LogInfo($"After patch total {recipeTable.RecipeDatas.Length} recipes ({RecipeLoader.Instance.newRecipes.Count} custom recipes)");

			uiRes._RecipeData = recipeTable;

			patchedSize = recipeTable.RecipeDatas.Length;
		}
	}
}
