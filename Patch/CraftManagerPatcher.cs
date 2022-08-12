using HarmonyLib;
using RF5.RecipeMod.Recipe;

namespace RF5.RecipeMod.Patch {
	[HarmonyPatch]
	internal class CraftManagerPatcher {
		private static int patchedSize = 0;
		public static bool farmPatched = false;

		[HarmonyPatch(typeof(CraftManager), nameof(CraftManager.CalcRecipeLearning))]
		[HarmonyPrefix]
		public static void FarmToSkillPatch() {
			var originalSize = CraftManager.FarmToolOutputItemIDtoSkillID.Count;
			if (originalSize == patchedSize) {
				return;
			}

			if (farmPatched) {
				Plugin.log.LogError($"Farm recipe skills already patched. Length: {originalSize}, should be: {patchedSize}");
			}
			farmPatched = true;
			Plugin.log.LogInfo($"Patching farm recipes");

			Plugin.log.LogInfo($"Before patch farm recipes {originalSize}");

			bool success = true;
			foreach (var itemID in RecipeLoader.Instance.newFarmRecipeIds) {
				if (!CraftManager.FarmToolOutputItemIDtoSkillID.ContainsKey(itemID)) {
					success &= CraftManager.FarmToolOutputItemIDtoSkillID.TryAdd(itemID, Define.SkillID.SKILL_FARM);
				}
			}
			if (!success) {
				Plugin.log.LogError($"Farm recipe patch failed");
			}
			Plugin.log.LogInfo($"After patch farm recipes {CraftManager.FarmToolOutputItemIDtoSkillID.Count}");

			patchedSize = CraftManager.FarmToolOutputItemIDtoSkillID.Count;
		}
	}
}
