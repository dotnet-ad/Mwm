namespace Mwm.AspNetCore
{
	using System;
	using Mwm.UI.Html;
	using Newtonsoft.Json;
	using System.Text;

	public class ValueSerializer : ISerializer
	{
		public object Deserialize(byte[] data, Type type)
		{
			var resolver = new ElementContractResolver();

			var jsonsettings = new JsonSerializerSettings()
			{
				ContractResolver = resolver,
			};

			var value = Encoding.UTF8.GetString(data);

			return JsonConvert.DeserializeObject(value, type, jsonsettings);
		}

		public byte[] Serialize(object value)
		{
			var resolver = new ElementContractResolver();

			var jsonsettings = new JsonSerializerSettings()
			{
				ContractResolver = resolver,
			};

			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, jsonsettings));
		}
	}
}
