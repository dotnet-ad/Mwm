namespace Mwm.UI.Html
{
	using System;
	using Newtonsoft.Json;

	public class VisibilityConverter : JsonConverter
	{
		public VisibilityConverter()
		{
		}

		public override bool CanConvert(Type objectType) => (objectType == typeof(Visibility));

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string result;

			switch ((Visibility)value)
			{
				case Visibility.Collapsed: result = "collapse"; break;
				default: result = "visible"; break;
			}

			writer.WriteValue(result);
		}
	}
}
