using HarmonyLib;
using RF5.RecipeMod.Recipe;
using SaveData;
using UnhollowerBaseLib;
using UnityEngine;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class RecipePatcher {
		//private static int patchedSize = 0;
		public static bool recipePatched = false;

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.RecipeData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void AddRecipe(UIRes __instance, ref RecipeDataTableArray __result) {
			var originalSize = __result.RecipeDatas.Length;
			//if (originalSize == patchedSize) {
			if (recipePatched) {
				return;
			}
			Plugin.log.LogInfo($"Patching Recipes");
			recipePatched = true;

			var loadedRecipes = RecipeLoader.Instance;
			var newTable = ScriptableObject.CreateInstance<RecipeDataTableArray>();

			//update recipe data
			var newRecipeData = new Il2CppReferenceArray<RecipeDataTableArray.RecipeDataTable>(originalSize + loadedRecipes.newRecipes.Count);
			for (int i = 0; i < originalSize; i++) {
				newRecipeData[i] = __result.RecipeDatas[i];
			}

			//insert new recipe here
			for (int i = originalSize; i < originalSize + loadedRecipes.newRecipes.Count; i++) {
				newRecipeData[originalSize] = loadedRecipes.newRecipes[i - originalSize];
			}

			newTable.RecipeDatas = newRecipeData;

			__result = newTable;
			__instance._RecipeData = newTable;

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
