using System.Collections.Generic;

namespace RF5.RecipeMod.Recipe {
	internal class CustomRecipeList {
		public List<CustomRecipe> Recipes { get; set; }

		public CustomRecipeList() {
		}

		public CustomRecipeList(List<CustomRecipe> recipes) {
			Recipes = recipes;
		}
	}
}
