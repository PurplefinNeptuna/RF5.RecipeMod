using RF5.RecipeMod.Recipe;

namespace RF5.RecipeMod.Patch {
	internal class CraftManagerPatch {
		private static int farmPatchedSize = 0;
		public static bool farmPatched = false;

		internal static void FarmToSkillPatch() {
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
