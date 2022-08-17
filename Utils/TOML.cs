using System.IO;
using Tomlet;
using Tomlet.Models;

namespace RF5.RecipeMod.Utils {
	internal class TOML {

		public static void WriteToFile(string path, object data) {
			TomlDocument tomlDoc = TomletMain.DocumentFrom(data);
			tomlDoc.ForceNoInline = true;
			File.WriteAllText(path, tomlDoc.SerializedValue);
		}

		public static T ReadFromFile<T>(string path) {
			return default;
		}
	}
}
