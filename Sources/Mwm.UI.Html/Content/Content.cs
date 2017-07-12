namespace Mwm.UI.Html
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Reflection;

	public static class Content
	{
		private static Dictionary<string, string> cache = new Dictionary<string, string>();

		public static string Load(string name)
		{
			if(cache.TryGetValue(name, out string result))
			{
				return result;
			}

			var assembly = typeof(Content).GetTypeInfo().Assembly;
			var resourceName = $"Mwm.UI.Html.Content.{name}";

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			using (var reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
				cache[name] = result;
				return result;
			}
		}
	}
}
