using HarmonyLib;
using SaveData;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace RF5.RecipeMod.Patches {
	[HarmonyPatch]
	internal class RecipePatcher {
		//private static int patchedSize = 0;
		public static bool recipePatched = false;

		public static int startRecipeIndex = 663;
		public static int currentRecipeIndex;

		public static List<RecipeDataTableArray.RecipeDataTable> newRecipes;
		public static Dictionary<CraftCategoryId, List<RecipeDataTableArray.RecipeDataTable>> newRecipeCategory;

		public static void InitRecipes() {
			currentRecipeIndex = startRecipeIndex;
			newRecipes = new();
			newRecipeCategory = new();
			for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
				newRecipeCategory.Add(i, new List<RecipeDataTableArray.RecipeDataTable>());
			}
		}

		public static void CreateRecipes() {
			//new recipe here
			RecipeDataTableArray.RecipeDataTable newRecipe = new();
			newRecipe.categoryId = CraftCategoryId.FarmTool;
			newRecipe.id = (RecipeId)currentRecipeIndex;
			newRecipe.RecipeRelease = (RecipeRelease)currentRecipeIndex;
			newRecipe.ResultItemId = ItemID.Item_Mushimegane;
			newRecipe.RpUse = 0;
			newRecipe.SkillLv = 30;
			newRecipe.SourceItems = new[] {
						ItemID.Item_Kuzutetsu
				};

			newRecipes.Add(newRecipe);
			newRecipeCategory[newRecipe.categoryId].Add(newRecipe);

			currentRecipeIndex++;
		}

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

			var newTable = ScriptableObject.CreateInstance<RecipeDataTableArray>();

			//update recipe data
			var newRecipeData = new Il2CppReferenceArray<RecipeDataTableArray.RecipeDataTable>(originalSize + newRecipes.Count);
			for (int i = 0; i < originalSize; i++) {
				newRecipeData[i] = __result.RecipeDatas[i];
			}

			//insert new recipe here
			for (int i = originalSize; i < originalSize + newRecipes.Count; i++) {
				newRecipeData[originalSize] = newRecipes[i - originalSize];
			}

			newTable.RecipeDatas = newRecipeData;

			__result = newTable;
			__instance._RecipeData = newTable;

			//patchedSize = originalSize + newRecipes.Count;
		}

		[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.CheckRecipeRelease))]
		[HarmonyPrefix]
		public static bool CheckRelease(RecipeRelease recipeId, ref bool __result) {
			if ((int)recipeId >= startRecipeIndex) {
				Plugin.log.LogInfo($"Skipped Check for {(int)recipeId}");
				__result = true;
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.SetRecipeRelease))]
		[HarmonyPrefix]
		public static bool SetRelease(RecipeRelease recipeId) {
			if ((int)recipeId >= startRecipeIndex) {
				Plugin.log.LogInfo($"Skipped Set for {(int)recipeId}");
				return false;
			}
			return true;
		}
	}
}
