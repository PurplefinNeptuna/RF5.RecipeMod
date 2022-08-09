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
		public Dictionary<CraftCategoryId, List<RecipeDataTableArray.RecipeDataTable>> newRecipeCategory;

		private RecipeLoader() {
			currentRecipeIndex = startRecipeIndex;
			newRecipes = new();
			newRecipeCategory = new();
			for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
				newRecipeCategory.Add(i, new List<RecipeDataTableArray.RecipeDataTable>());
			}
		}

		public void LoadRecipes() {
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
	}
}
