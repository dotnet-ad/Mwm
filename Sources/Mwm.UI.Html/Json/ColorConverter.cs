namespace Mwm.UI.Html
{
	using System;
	using Newtonsoft.Json;

	public class ColorConverter : JsonConverter
	{
		public ColorConverter()
		{
		}

		public override bool CanConvert(Type objectType) => (objectType == typeof(Color));

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var color = ((Color)value);
			writer.WriteValue($"rgba({color.R}, {color.G}, {color.B}, {(color.A / 255.0f)})");
		}
	}
}
