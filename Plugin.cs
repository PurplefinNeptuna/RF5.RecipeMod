using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using RF5.RecipeMod.Patch;
using RF5.RecipeMod.Recipe;
using System.IO;
using System.Reflection;

namespace RF5.RecipeMod {
	[BepInPlugin(GUID, MODNAME, VERSION)]
	internal class Plugin : BasePlugin {

		public const string MODNAME = "RecipeMod";
		public const string GUID = "RF5.RecipeMod";
		public const string VERSION = "1.0.0";

		internal static ManualLogSource log;
		internal static string FILEPATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public override void Load() {
			log = Logger.CreateLogSource("RecipeMod");
			// Plugin startup logic
			log.LogInfo($"Plugin {GUID} is loaded!");

#if DEBUG
			RecipeLoader.Instance.CreateDebugRecipe();
#endif
			RecipeLoader.Instance.LoadRecipes();

			Harmony.CreateAndPatchAll(typeof(SVPatch));
			Harmony.CreateAndPatchAll(typeof(RecipePatch));
			Harmony.CreateAndPatchAll(typeof(CategoryPatch));
			Harmony.CreateAndPatchAll(typeof(SaveDataPatch));
		}
	}
}
