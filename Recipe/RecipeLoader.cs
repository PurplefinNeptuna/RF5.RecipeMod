using RF5.RecipeMod.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

		public void CreateExampleRecipes() {
			List<CustomRecipe> clipperAndMagnifier = new() {
				new((RecipeId)663) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Kegaribasami,
					SkillLevel = 10,
					IngredientItemIDs = new() {
						ItemID.Item_Tetsu,
						ItemID.Item_Tetsu,
					},
				},
				new((RecipeId)664) {
					CraftCategoryID = CraftCategoryId.FarmTool,
					ResultItemID = ItemID.Item_Mushimegane,
					SkillLevel = 10,
					IngredientItemIDs = new() {
						ItemID.Item_Kuzutetsu,
						ItemID.Item_Sugoikuzutetsu,
					},
					RecipeReleaseID = RecipeRelease.Recipe_Farm_1_L,
				},
			};

			List<CustomRecipe> largeCrystals = new() {
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

			var clipperName = Path.Combine(customRecipeFolder, "ClipAndMag.json");
			var crystalName = Path.Combine(customRecipeFolder, "LargeCrystals.json");

			JSON.WriteToFile(clipperName, clipperAndMagnifier);
			JSON.WriteToFile(crystalName, largeCrystals);

			var clipperToml = Path.Combine(customRecipeFolder, "ClipAndMag.toml");
			var crystalToml = Path.Combine(customRecipeFolder, "LargeCrystals.toml");

			TOML.WriteToFile(clipperToml, new CustomRecipeList(clipperAndMagnifier));
			TOML.WriteToFile(crystalToml, new CustomRecipeList(largeCrystals));
		}

		public void LoadRecipes() {
			//list all json files
			List<string> fileList = Directory.GetFiles(customRecipeFolder, "*.json", SearchOption.AllDirectories).ToList();
			if(fileList.Count <= 0) {
				Plugin.log.LogInfo("No recipes found, creating example recipes");
				CreateExampleRecipes();
				fileList = Directory.GetFiles(customRecipeFolder, "*.json", SearchOption.AllDirectories).ToList();
			}
			Plugin.log.LogInfo("Loading custom recipes");

			//read the array in each files
			List<CustomRecipe> customRecipes = new();
			foreach (string file in fileList) {
				List<CustomRecipe> newCustomRecipes = JSON.ReadFromFile<List<CustomRecipe>>(file);

				Plugin.log.LogInfo($"Got {newCustomRecipes.Count} new recipes from {Path.GetFileName(file)}");

				customRecipes.AddRange(newCustomRecipes);
			}

			foreach (CustomRecipe customRecipe in customRecipes) {
				//new recipe here
				var newRecipe = (RecipeDataTableArray.RecipeDataTable)customRecipe;

				//add the recipe to collections
				newRecipes.Add(newRecipe);
				newRecipeCategories[newRecipe.categoryId].Add(newRecipe.id);

				//additional checking for FarmTools
				if (newRecipe.categoryId == CraftCategoryId.FarmTool) {
					newFarmRecipeIds.Add(newRecipe.ResultItemId);
				}
			}
		}
	}
}
