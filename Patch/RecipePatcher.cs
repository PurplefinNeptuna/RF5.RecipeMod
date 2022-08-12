using HarmonyLib;
using RF5.RecipeMod.Recipe;
using SaveData;
using System.Linq;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class RecipePatcher {
		private static int patchedSize = 0;
		public static bool recipePatched = false;

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.RecipeData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void AddRecipe(UIRes __instance, ref RecipeDataTableArray __result) {
			var originalSize = __result.RecipeDatas.Length;
			if (originalSize == patchedSize) {
				return;
			}

			if (recipePatched) {
				Plugin.log.LogError($"Recipe already patched\nLength: {originalSize}\nShould be: {patchedSize}");
			}

			Plugin.log.LogInfo($"Patching recipes");
			recipePatched = true;

			Plugin.log.LogInfo($"Before patch total {originalSize} recipes");

			__result.RecipeDatas = __result.RecipeDatas.Concat(RecipeLoader.Instance.newRecipes).ToArray();

			Plugin.log.LogInfo($"After patch total {__result.RecipeDatas.Length} recipes ({RecipeLoader.Instance.newRecipes.Count} custome recipes added)");

			__instance._RecipeData = __result;

			patchedSize = __result.RecipeDatas.Length;
		}
	}
}
