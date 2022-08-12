﻿using HarmonyLib;
using RF5.RecipeMod.Recipe;
using SaveData;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class SaveDataPatcher {
		[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.CheckRecipeRelease))]
		[HarmonyPrefix]
		public static bool CheckRelease(RecipeRelease recipeId, ref bool __result) {
			if ((int)recipeId >= RecipeLoader.Instance.startRecipeIndex) {
				Plugin.log.LogInfo($"Skipped check for {(int)recipeId}");
				__result = true;
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.SetRecipeRelease))]
		[HarmonyPrefix]
		public static bool SetRelease(RecipeRelease recipeId) {
			if ((int)recipeId >= RecipeLoader.Instance.startRecipeIndex) {
				Plugin.log.LogInfo($"Skipped set for {(int)recipeId}");
				return false;
			}
			return true;
		}
	}
}
