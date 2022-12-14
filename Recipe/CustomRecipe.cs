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
		public CustomRecipe() {
			RecipeLoader loader = RecipeLoader.Instance;
			RecipeID = (RecipeId)loader.currentRecipeIndex;
			if (RecipeReleaseID == RecipeRelease.Relase_EMPTY) {
				RecipeReleaseID = (RecipeRelease)loader.currentRecipeIndex;
			}
			loader.currentRecipeIndex++;
		}

		public static explicit operator RecipeDataTableArray.RecipeDataTable(CustomRecipe recipe) {
			var recipeData = new RecipeDataTableArray.RecipeDataTable {
				categoryId = recipe.CraftCategoryID,
				id = recipe.RecipeID,
				RecipeRelease = recipe.RecipeReleaseID,
				ResultItemId = recipe.ResultItemID,
				RpUse = 0,
				SkillLv = recipe.SkillLevel,
				SourceItems = recipe.IngredientItemIDs.ToArray()
			};
			return recipeData;
		}
	}
}
