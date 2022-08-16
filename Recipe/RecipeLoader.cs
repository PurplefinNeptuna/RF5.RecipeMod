using RF5.RecipeMod.Utils;
using System.Collections.Generic;
using System.IO;

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

		public string customRecipeFolder;

		private RecipeLoader() {
			currentRecipeIndex = startRecipeIndex;
			newRecipes = new();
			newRecipeCategories = new();
			newFarmRecipeIds = new();
			for (var i = CraftCategoryId.EMPTY; i < CraftCategoryId.Max; i++) {
				newRecipeCategories.Add(i, new List<RecipeId>());
			}
			customRecipeFolder = Path.Combine(Plugin.FILEPATH, $"Custom Recipes");
			Directory.CreateDirectory(customRecipeFolder);
		}

		public void CreateDebugRecipe() {
			List<CustomRecipe> customRecipes = new() {
				new((RecipeId)663) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Mushimegane,
					SkillLevel = 40,
					IngredientItemIDs = new() {
						ItemID.Item_Kuzutetsu,
						ItemID.Item_Sugoikuzutetsu,
					}
				},
				new((RecipeId)664) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Kegaribasami,
					SkillLevel = 40,
					IngredientItemIDs = new() {
						ItemID.Item_Tetsu,
						ItemID.Item_Tetsu,
					}
				},
				new((RecipeId)665) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Cry_tuchi,
					SkillLevel = 50,
					IngredientItemIDs = new() {
						ItemID.Item_Cryfragment_tuchi,
						ItemID.Item_Cryfragment_tuchi,
						ItemID.Item_Cryfragment_tuchi,
						ItemID.Item_Cryfragment_tuchi,
						ItemID.Item_Cryfragment_tuchi,
					}
				},
				new((RecipeId)666) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Cry_hi,
					SkillLevel = 50,
					SkillUnlockID = Define.SkillID.SKILL_FIRE,
					IngredientItemIDs = new() {
						ItemID.Item_Cryfragment_hi,
						ItemID.Item_Cryfragment_hi,
						ItemID.Item_Cryfragment_hi,
						ItemID.Item_Cryfragment_hi,
						ItemID.Item_Cryfragment_hi,
					}
				},
				new((RecipeId)667) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Cry_koori,
					SkillLevel = 50,
					SkillUnlockID = Define.SkillID.SKILL_WATER,
					IngredientItemIDs = new() {
						ItemID.Item_Cryfragment_koori,
						ItemID.Item_Cryfragment_koori,
						ItemID.Item_Cryfragment_koori,
						ItemID.Item_Cryfragment_koori,
						ItemID.Item_Cryfragment_koori,
					}
				},
				new((RecipeId)668) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Cry_kaze,
					SkillLevel = 50,
					SkillUnlockID = Define.SkillID.SKILL_WIND,
					IngredientItemIDs = new() {
						ItemID.Item_Cryfragment_kaze,
						ItemID.Item_Cryfragment_kaze,
						ItemID.Item_Cryfragment_kaze,
						ItemID.Item_Cryfragment_kaze,
						ItemID.Item_Cryfragment_kaze,
					}
				},
				new((RecipeId)669) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Cry_ti,
					SkillLevel = 50,
					SkillUnlockID = Define.SkillID.SKILL_EARTH,
					IngredientItemIDs = new() {
						ItemID.Item_Cryfragment_ti,
						ItemID.Item_Cryfragment_ti,
						ItemID.Item_Cryfragment_ti,
						ItemID.Item_Cryfragment_ti,
						ItemID.Item_Cryfragment_ti,
					}
				}
			};

			var jsonFileName = Path.Combine(customRecipeFolder, $"examples.json");
			//File.WriteAllText(jsonFileName, JsonConvert.SerializeObject(customRecipes));
			JSON.WriteToFile(jsonFileName, customRecipes);
		}

		public void LoadRecipes() {
			currentRecipeIndex = startRecipeIndex;

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
