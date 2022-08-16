using Define;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace RF5.RecipeMod.Recipe {
	internal class CustomRecipe {
		[JsonIgnore]
		public RecipeId RecipeID { get; set; }

		[JsonProperty(Required = Required.Always)]
		public CraftCategoryId CraftCategoryID { get; set; }

		[JsonProperty(Required = Required.Always)]
		public ItemID ResultItemID { get; set; }

		[DefaultValue(5)]
		public int SkillLevel { get; set; } = 5;

		[DefaultValue(SkillID.SKILL_FARM)]
		public SkillID SkillUnlockID { get; set; } = SkillID.SKILL_FARM;

		[JsonProperty(Required = Required.Always)]
		public List<ItemID> IngredientItemIDs { get; set; }

		public RecipeRelease RecipeReleaseID { get; set; }

		public CustomRecipe(RecipeId recipeID, RecipeRelease recipeRelease = RecipeRelease.Relase_EMPTY) {
			this.RecipeID = recipeID;
			if (recipeRelease == RecipeRelease.Relase_EMPTY) {
				recipeRelease = (RecipeRelease)recipeID;
			}
			this.RecipeReleaseID = recipeRelease;
		}

		[JsonConstructor]
		public CustomRecipe(CraftCategoryId craftCategoryID, ItemID resultItemID, List<ItemID> ingredientItemIDs) {
			this.CraftCategoryID = craftCategoryID;
			this.ResultItemID = resultItemID;
			this.IngredientItemIDs = ingredientItemIDs;

			RecipeLoader loader = RecipeLoader.Instance;
			RecipeID = (RecipeId)loader.currentRecipeIndex;
			if (RecipeReleaseID == RecipeRelease.Relase_EMPTY) {
				RecipeReleaseID = (RecipeRelease)loader.currentRecipeIndex;
			}
			loader.currentRecipeIndex++;
		}
	}
}
