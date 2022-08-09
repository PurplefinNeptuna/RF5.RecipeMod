using HarmonyLib;
using RF5.RecipeMod.Recipe;
using SaveData;
using System.Linq;
using UnhollowerBaseLib;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class RecipePatcher {
		//private static int patchedSize = 0;
		public static bool recipePatched = false;

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.RecipeData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void AddRecipe(UIRes __instance, ref RecipeDataTableArray __result) {
			//if (originalSize == patchedSize) {
			if (recipePatched) {
				//Plugin.log.LogInfo($"Recipe already patched, Length: {__result.RecipeDatas.Length}");
				return;
			}
			Plugin.log.LogInfo($"Patching Recipes");
			recipePatched = true;

			var loadedRecipes = RecipeLoader.Instance;

			Plugin.log.LogInfo($"Before Patch Recipes {__result.RecipeDatas.Length}");

			__result.RecipeDatas = __result.RecipeDatas.Concat(loadedRecipes.newRecipes).ToArray();

			Plugin.log.LogInfo($"After Patch Recipes {__result.RecipeDatas.Length}");

			__instance._RecipeData = __result;

			//patchedSize = originalSize + newRecipes.Count;
		}

		[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.CheckRecipeRelease))]
		[HarmonyPrefix]
		public static bool CheckRelease(RecipeRelease recipeId, ref bool __result) {
			if ((int)recipeId >= RecipeLoader.Instance.startRecipeIndex) {
				Plugin.log.LogInfo($"Skipped Check for {(int)recipeId}");
				__result = true;
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.SetRecipeRelease))]
		[HarmonyPrefix]
		public static bool SetRelease(RecipeRelease recipeId) {
			if ((int)recipeId >= RecipeLoader.Instance.startRecipeIndex) {
				Plugin.log.LogInfo($"Skipped Set for {(int)recipeId}");
				return false;
			}
			return true;
		}
	}
}
