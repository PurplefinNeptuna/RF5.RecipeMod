using System.IO;
using System.Linq;

namespace RF5.RecipeMod.Utils {
	internal static class DebugPrinter {

		public static void PrintRecipes(RecipeDataTableArray recipeDataTableArray, string name = "RecipeTable") {
			var jsonFileName = Path.Combine(Plugin.FILEPATH, $"{name}.json");
			JSON.WriteToFile(jsonFileName, recipeDataTableArray.RecipeDatas.ToArray());
		}

		public static void PrintCraftCategories(CraftCategoryDataTable craftCategoryDataTable, string name = "CraftCategoryTable") {
			var jsonFileName = Path.Combine(Plugin.FILEPATH, $"{name}.json");
			JSON.WriteToFile(jsonFileName, craftCategoryDataTable.CraftCategoryDatas.ToArray());
		}
	}
}
