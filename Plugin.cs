using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using RF5.RecipeMod.Utils;
using SaveData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnhollowerBaseLib;
using UnityEngine;

namespace RF5.RecipeMod {
	[BepInPlugin(GUID, MODNAME, VERSION)]
	internal class Plugin : BasePlugin {

		public const string MODNAME = "RecipeMod";
		public const string GUID = "RF5.RecipeMod";
		public const string VERSION = "1.0.0";

		internal static ManualLogSource log;

		public static string FILEPATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public static bool recipePatched = false;
		public static bool categoryPatched = false;

		public static int startRecipeIndex = 663;
		public static int currentRecipeIndex;

		public static List<RecipeDataTableArray.RecipeDataTable> newRecipes;
		public static Dictionary<CraftCategoryId, List<RecipeDataTableArray.RecipeDataTable>> newRecipeCategory;

		public override void Load() {
			log = Log;
			// Plugin startup logic
			log.LogInfo($"Plugin {GUID} is loaded!");

			InitRecipes();
			CreateRecipes();

			Harmony.CreateAndPatchAll(typeof(SVPatcher));
			Harmony.CreateAndPatchAll(typeof(RecipePatcher));
			Harmony.CreateAndPatchAll(typeof(CategoryPatcher));
		}

		public void InitRecipes() {
			currentRecipeIndex = startRecipeIndex;
			newRecipes = new();
			newRecipeCategory = new();
			for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
				newRecipeCategory.Add(i, new List<RecipeDataTableArray.RecipeDataTable>());
			}
		}

		public void CreateRecipes() {
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

		[HarmonyPatch]
		internal class SVPatcher {
			[HarmonyPatch(typeof(SV), nameof(SV.CreateUIRes))]
			[HarmonyPostfix]
			public static void SVCreateUIResPatch() {
				recipePatched = false;
				categoryPatched = false;

#if DEBUG
				DebugPrinter.PrintRecipes(SV.UIRes.RecipeData, "RecipeTable_onInit");
				DebugPrinter.PrintCraftCategories(SV.UIRes.CraftCategoryData, "CraftCategoryTable_onInit");
#endif
			}
		}

		[HarmonyPatch]
		internal class RecipePatcher {
			//private static int patchedSize = 0;

			[HarmonyPatch(typeof(UIRes), nameof(UIRes.RecipeData), MethodType.Getter)]
			[HarmonyPostfix]
			public static void AddRecipe(UIRes __instance, ref RecipeDataTableArray __result) {
				var originalSize = __result.RecipeDatas.Length;
				//if (originalSize == patchedSize) {
				if (recipePatched) {
					return;
				}
				log.LogInfo($"Patching Recipes");
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
					log.LogInfo($"Skipped Check for {(int)recipeId}");
					__result = true;
					return false;
				}
				return true;
			}

			[HarmonyPatch(typeof(RF5ItemFlagData), nameof(RF5ItemFlagData.SetRecipeRelease))]
			[HarmonyPrefix]
			public static bool SetRelease(RecipeRelease recipeId) {
				if ((int)recipeId >= startRecipeIndex) {
					log.LogInfo($"Skipped Set for {(int)recipeId}");
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch]
		internal class CategoryPatcher {
			//private static List<int> patchedSize = new();

			public static bool SameSize(List<int> origSize, List<int> patchSize) {
				if (origSize.Count != patchSize.Count) return false;
				for (int i = 0; i < origSize.Count; i++) {
					if (origSize[i] != patchSize[i]) return false;
				}
				return true;
			}

			[HarmonyPatch(typeof(UIRes), nameof(UIRes.CraftCategoryData), MethodType.Getter)]
			[HarmonyPostfix]
			public static void AddCategoryRecipe(UIRes __instance, ref CraftCategoryDataTable __result) {
				var originalSize = __result.CraftCategoryDatas.Select(x => x.RecipeIds.Length).ToList();
				//if (SameSize(originalSize, patchedSize)) {
				if (categoryPatched) {
					return;
				}
				log.LogInfo($"Patching Category Recipes");
				categoryPatched = true;

				var newSize = new List<int>(originalSize);

				//insert new recipe to categories
				for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
					newSize[(int)i] += newRecipeCategory[i].Count;
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
						for (int k = 0; k < newRecipeCategory[(CraftCategoryId)i].Count; k++) {
							newCategoryRecipes[originalSize[i] + k] = newRecipeCategory[(CraftCategoryId)i][k].id;
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
}
