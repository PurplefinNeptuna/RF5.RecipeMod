# Recipe Mod for Rune Factory 5
Rune Factory 5 BepInEx plugin for loading custom recipes.

## Installation
1. Install [RF5Fix mod](https://github.com/Lyall/RF5Fix) (BepInEx included there).
2. Download this mod [here.](https://github.com/PurplefinNeptuna/RF5.RecipeMod/releases)
3. Extract RF5.RecipeMod.zip file to `<GameDirectory>\BepInEx\plugins`.
4. (*Recommended*) Make folder for each plugins so it will be easier to manage plugins.  

![extract-location](https://user-images.githubusercontent.com/23467102/184843956-07f70c2f-2088-440f-ab0e-390f5e986a5d.png)
![plugin-inside-each-folder](https://user-images.githubusercontent.com/23467102/184843357-2c22a72c-cd37-40c8-b518-3afe6e1ea361.png)

## What this mod do
Load custom recipes data (in JSON format) from `Custom Recipes` folder.  
This mod has 2 custom recipes files included
- `ClipAndMag.json` contains recipes for clipper and magnifying glass
- `LargeCrystals.json` contains recipes for all large crystals  

![ClipAndMag](https://user-images.githubusercontent.com/23467102/184844622-b5fb636e-6818-4b39-8992-9680ac1af859.png)
![LargeCystals](https://user-images.githubusercontent.com/23467102/184845039-88475772-538e-4ae8-9d11-b621b8040afb.png)

## (Advanced) Create custom recipes files
You can create your own custom recipes by creating new JSON file inside `Custom Recipes` folder (e.g. `MyCustomRecipes.json`).
### **JSON File Structure**
Inside your new JSON file:
```js
[
	{
		//Recipe 1
	},
	{
		//Recipe 2
	},
	{
		//Recipe 3 and so on
	}
]
```
Remember that JSON file doesn't support comments, it is used now just for visualization.
Also don't forget to remove any trailing commas.  
Basically, your file contain **an array of `CustomRecipe` objects**.
### **`CustomeRecipe` object**
`CustomRecipe` contain these properties:
Key|Requirement|Value|Example|Definition|Default Value
---|---|---|---|---|---
`"CraftCategoryID"`|Required|`CraftCategoryId` or `int`|`"Drug"` or `8`|Recipe's category|None
`"ResultItemID"`|Required|`ItemID` or `int`|`"Item_Kin"` or `2156`|Result item|None
`"IngredientItemIDs"`|Required|Array of `ItemID` or `int`|`["Item_Gin", "Item_Gin"]` or `[2155, 2155]`|Item used for recipe, max 6 items|None
`"RecipeReleaseID"`|Optional|`RecipeRelease` or `int`|`"Recipe_Drag_1_L"` or `149`|Condition for recipe unlocks, recipe always unlocked if not defined|Internally set
`"SkillLevel"`|Optional|`int`|`25`|Used for RP usage|`5`
`"SkillUnlockID"`|Optional|`SkillID` or `int`|`"SKILL_MIX"` or `20`|(Currently Unused)|`SKILL_FARM`

The properties doesn't need to be in order, but Required properties must always exist.  
[List of CraftCategoryId values](https://github.com/SinsofSloth/RF5-global-metadata/blob/main/_no_namespace/CraftCategoryId.cs)  
[List of ItemID values](https://docs.google.com/spreadsheets/d/1LFkOEVQuJ-x1Lkn64Lt8Z7NvbVyUgqHP794Ehw3wbts/edit#gid=920483090)  
[List of RecipeRelease values](https://docs.google.com/spreadsheets/d/1LFkOEVQuJ-x1Lkn64Lt8Z7NvbVyUgqHP794Ehw3wbts/edit#gid=914955553)
### **Custom JSON file example**
```js
[
	{
		"CraftCategoryID": "Drug",
		"ResultItemID": "Item_Kin",
		"IngredientItemIDs": [
			"Item_Gin",
			"Item_Gin"
		],
		"RecipeReleaseID": "Recipe_Drag_1_L",
		"SkillLevel": 25,
		"SkillUnlockID": "SKILL_MIX"
	},
	{
		"CraftCategoryID": 8,
		"ResultItemID": 2157,
		"SkillLevel": 25,
		"IngredientItemIDs": [
			2156,
			2156
		]
	},
	{
		"CraftCategoryID": 8,
		"ResultItemID": 2154,
		"IngredientItemIDs": [
			2152,
			2152
		]
	}
]
```
First recipe is recipe to create Gold using 2 Silvers, most values are enum string.  
Second recipe is recipe to create Platinum using 2 Gold, all values are int.  
Third recipe is recipe to create Bronze using 2 Iron, created with only the required parameters.