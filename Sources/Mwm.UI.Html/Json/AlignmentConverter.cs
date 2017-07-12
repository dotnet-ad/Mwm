namespace Mwm.UI.Html
{
	using System;
	using Newtonsoft.Json;

	public class AlignmentConverter : JsonConverter
	{
		public AlignmentConverter()
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

			switch ((Alignment)value)
			{
				case Alignment.Start: result = "flex-start"; break;
				case Alignment.End: result = "flex-end"; break;
				case Alignment.Center: result = "center"; break;
				default: result = "stretch"; break;
			}

			writer.WriteValue(result);
		}
	}
}
