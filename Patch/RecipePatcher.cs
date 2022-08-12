using HarmonyLib;
using RF5.RecipeMod.Recipe;
using System.Linq;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class RecipePatcher {
		private static int patchedSize = 0;
		public static bool recipePatched = false;

		private static int farmPatchedSize = 0;
		public static bool farmPatched = false;

		[HarmonyPatch(typeof(UIRes), nameof(UIRes.RecipeData), MethodType.Getter)]
		[HarmonyPostfix]
		public static void RecipeDataPatch(UIRes __instance, ref RecipeDataTableArray __result) {
			AddRecipe(__instance, ref __result);
			FarmToSkillPatch();
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

		private static void FarmToSkillPatch() {
			var originalSize = CraftManager.FarmToolOutputItemIDtoSkillID.Count;
			if (originalSize == farmPatchedSize) {
				return;
			}

			if (farmPatched) {
				Plugin.log.LogError($"Farm item recipe already patched. Length: {originalSize}, should be: {farmPatchedSize}");
			}
			farmPatched = true;
			Plugin.log.LogInfo($"Patching farm item recipes");

			Plugin.log.LogInfo($"Before patch farm total {originalSize} item recipes");

			bool success = true;
			foreach (var itemID in RecipeLoader.Instance.newFarmRecipeIds) {
				if (!CraftManager.FarmToolOutputItemIDtoSkillID.ContainsKey(itemID)) {
					success &= CraftManager.FarmToolOutputItemIDtoSkillID.TryAdd(itemID, Define.SkillID.SKILL_FARM);
				}
			}
			if (!success) {
				Plugin.log.LogError($"Farm recipe item patch failed");
			}
			Plugin.log.LogInfo($"After patch farm total {CraftManager.FarmToolOutputItemIDtoSkillID.Count} item recipes ({RecipeLoader.Instance.newFarmRecipeIds.Count} custom farm item recipes)");

			farmPatchedSize = CraftManager.FarmToolOutputItemIDtoSkillID.Count;
		}
	}
}
