using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RF5.RecipeMod.Recipe;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RF5.RecipeMod.Utils {
	internal static class JSON {

		public static void WriteToFile(string path, object data) {
			using (var fs = File.Create(path))
			using (var sw = new StreamWriter(fs))
			using (var jtw = new JsonTextWriter(sw) {
				Formatting = Formatting.Indented,
				Indentation = 1,
				IndentChar = '\t'
			}) {
				var serializer = new CustomSerializer();
				serializer.Serialize(jtw, data);
			}
		}

		public static T ReadFromFile<T>(string path) {
			using (var fs = File.Open(path, FileMode.Open))
			using (var sr = new StreamReader(fs))
			using (var jtr = new JsonTextReader(sr)) {
				var serializer = new CustomSerializer();
				return (T)serializer.Deserialize(jtr, typeof(T));
			}
		}

		internal class CustomSerializer : JsonSerializer {
			public CustomSerializer() {
				DefaultValueHandling = DefaultValueHandling.Ignore;
				PreserveReferencesHandling = PreserveReferencesHandling.None;
				MissingMemberHandling = MissingMemberHandling.Ignore;
				ContractResolver = new CustomResolver();
				Converters.Add(new StringEnumConverter { AllowIntegerValues = true });
				Error += delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) {
					Plugin.log.LogError(args.ErrorContext.Error.Message);
					args.ErrorContext.Handled = true;
				};
			}
		}

		internal class CustomResolver : DefaultContractResolver {
			private readonly HashSet<string> ignoreProps;
			public CustomResolver() {
				//this is for Il2Cpp objects
				this.ignoreProps = new HashSet<string>() {
				"ObjectClass",
				"Pointer"
				};
			}

			protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
				JsonProperty property = base.CreateProperty(member, memberSerialization);
				//check for Il2Cpp
				if (this.ignoreProps.Contains(property.PropertyName)) {
					property.ShouldSerialize = _ => false;
				}
				//for CustomRecipe ignore if releaseID == recipeID
				else if (property.DeclaringType == typeof(CustomRecipe) && property.PropertyName == nameof(CustomRecipe.RecipeReleaseID)) {
					property.ShouldSerialize =
					instance => {
						CustomRecipe i = (CustomRecipe)instance;
						return (int)i.RecipeReleaseID != (int)i.RecipeID;
					};
				}
				return property;
			}
		}
	}
}
