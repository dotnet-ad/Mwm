namespace Mwm.UI.Html
{
	using System;
	using Newtonsoft.Json;

	public class OrientationConverter : JsonConverter
	{
		public OrientationConverter()
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

			switch ((Orientation)value)
			{
				case Orientation.Horizontal: result = "row"; break;
				default: result = "column"; break;
			}

			writer.WriteValue(result);
		}
	}
}
