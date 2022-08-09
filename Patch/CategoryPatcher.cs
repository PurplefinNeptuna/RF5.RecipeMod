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

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.CraftCategoryData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void AddCategoryRecipe(UIRes __instance, ref CraftCategoryDataTable __result) {
			var originalSize = __result.CraftCategoryDatas.Select(x => x.RecipeIds.Length).ToList();
			//if (SameSize(originalSize, patchedSize)) {
			if (categoryPatched) {
				return;
			}
			Plugin.log.LogInfo($"Patching Category Recipes");
			categoryPatched = true;

			var loadedRecipes = RecipeLoader.Instance;
			var newSize = new List<int>(originalSize);

			//insert new recipe to categories
			for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
				newSize[(int)i] += loadedRecipes.newRecipeCategory[i].Count;
			}

			var newTable = ScriptableObject.CreateInstance<CraftCategoryDataTable>();
			var newCategoryData = new Il2CppReferenceArray<CraftCategoryDataTable.CraftCategoryData>(originalSize.Count);
			for (int i = 0; i < originalSize.Count; i++) {
				if (originalSize[i] == newSize[i]) {
					newCategoryData[i] = __result.CraftCategoryDatas[i];
				}
				else {
					//update category recipe data
					var newCategoryRecipes = new Il2CppStructArray<RecipeId>(newSize[i]);
					for (int j = 0; j < originalSize[i]; j++) {
						newCategoryRecipes[j] = __result.CraftCategoryDatas[i].RecipeIds[j];
					}

					//insert the new recipes here
					for (int k = 0; k < loadedRecipes.newRecipeCategory[(CraftCategoryId)i].Count; k++) {
						newCategoryRecipes[originalSize[i] + k] = loadedRecipes.newRecipeCategory[(CraftCategoryId)i][k].id;
					}

					CraftCategoryDataTable.CraftCategoryData newCategory = new();
					newCategory.SkillID = __result.CraftCategoryDatas[i].SkillID;
					newCategory.RecipeIds = newCategoryRecipes;

					newCategoryData[i] = newCategory;
				}
			}
			newTable.CraftCategoryDatas = newCategoryData;

			__result = newTable;
			__instance._CraftCategoryDataTable = newTable;

			//patchedSize = new(newSize);
		}
	}
}
