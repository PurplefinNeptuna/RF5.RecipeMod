using System.Collections.Generic;

namespace RF5.RecipeMod.Recipe {
	internal class RecipeLoader {

		#region Singleton
		private static readonly RecipeLoader instance = new();

		static RecipeLoader() {
		}

		public static RecipeLoader Instance {
			get {
				return instance;
			}
		}
		#endregion

		public int startRecipeIndex = 663;
		public int currentRecipeIndex;

		public List<RecipeDataTableArray.RecipeDataTable> newRecipes;
		public Dictionary<CraftCategoryId, List<RecipeId>> newRecipeCategories;
		public List<ItemID> newFarmRecipeIds;

		private RecipeLoader() {
			currentRecipeIndex = startRecipeIndex;
			newRecipes = new();
			newRecipeCategories = new();
			newFarmRecipeIds = new();
			for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
				newRecipeCategories.Add(i, new List<RecipeId>());
			}
		}

		public void LoadRecipes() {
			//new recipe here, could use some kind of file reader later
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

			//add the recipe to collections
			newRecipes.Add(newRecipe);
			newRecipeCategories[newRecipe.categoryId].Add(newRecipe.id);

			//additional checking for FarmTools
			if (newRecipe.categoryId == CraftCategoryId.FarmTool) {
				newFarmRecipeIds.Add(newRecipe.ResultItemId);
			}

			currentRecipeIndex++;
		}
	}
}
