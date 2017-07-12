namespace Mwm.UI.Html
{
	using System;
	using Newtonsoft.Json;

	public class ThicknessConverter : JsonConverter
	{
		public ThicknessConverter()
		{
		}

		public override bool CanConvert(Type objectType) => (objectType == typeof(Visibility));

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var t = ((Thickness)value);
			writer.WriteValue($"{t.Top}px {t.Right}px {t.Bottom}px {t.Left}px");
		}
	}
}
