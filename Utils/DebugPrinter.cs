using System.IO;
using System.Linq;

namespace RF5.RecipeMod.Utils {
	internal static class DebugPrinter {

		public static string FILEPATH => Plugin.FILEPATH;

		public static void PrintRecipes(RecipeDataTableArray recipeDataTableArray, string name = "RecipeTable") {
			var fileName = Path.Combine(FILEPATH, $"{name}.txt");

			using (StreamWriter sw = new(fileName, false)) {
				foreach (var (recipeData, i) in recipeDataTableArray.RecipeDatas.Select((v, k) => (v, k))) {
					string recipeString = $"Recipe : {i}";
					recipeString += $"\n\tID: {recipeData.id} at {(int)recipeData.id}";
					recipeString += $"\n\tCategory: {recipeData.categoryId}";
					recipeString += $"\n\tResult Item: {recipeData.ResultItemId}";
					recipeString += $"\n\tRequired Item:";
					foreach (var (recipeItem, idx) in recipeData.SourceItems.Select((v, k) => (v, k))) {
						recipeString += $"\n\t\tRequired Item {idx}: {recipeItem}";
					}
					recipeString += $"\n\tSkill Required: {recipeData.SkillLv}";
					recipeString += $"\n\tRp Required: {recipeData.RpUse}";
					recipeString += $"\n\tRecipe Unlock: {recipeData.RecipeRelease}";
					sw.WriteLine(recipeString);
				}
			}
		}

		public static void PrintCraftCategories(CraftCategoryDataTable craftCategoryDataTable, string name = "CraftCategoryTable") {
			var fileName = Path.Combine(FILEPATH, $"{name}.txt");

			using (StreamWriter sw = new(fileName, false)) {
				foreach (var (categoryData, i) in craftCategoryDataTable.CraftCategoryDatas.Select((v, k) => (v, k))) {
					string categoryString = $"Category : {i}";
					categoryString += $"\n\tID: {(CraftCategoryId)i}";
					categoryString += $"\n\tSkill ID: {categoryData.SkillID}";
					categoryString += $"\n\tRecipe List:";
					foreach (var (recipeID, idx) in categoryData.RecipeIds.Select((v, k) => (v, k))) {
						categoryString += $"\n\t\tRecipe {idx}: {recipeID}";
					}
					sw.WriteLine(categoryString);
				}
			}
		}
	}
}
