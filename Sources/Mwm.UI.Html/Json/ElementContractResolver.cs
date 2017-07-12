using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Mwm.UI.Html
{
	public class ElementContractResolver : DefaultContractResolver
	{
		#region Default

		private static readonly Lazy<ElementContractResolver> instance = new Lazy<ElementContractResolver>(() => new ElementContractResolver());

		public static ElementContractResolver Default => instance.Value;

		#endregion

		public static readonly Dictionary<Type, JsonConverter> Converters = new Dictionary<Type, JsonConverter>()
		{
			{typeof(Color), new ColorConverter()},
			{typeof(Visibility), new VisibilityConverter()},
			{typeof(Thickness), new ThicknessConverter()},
			{typeof(Orientation), new OrientationConverter()},
			{typeof(Alignment), new AlignmentConverter()},
		};

		protected override JsonContract CreateContract(Type objectType)
		{
			var contract = base.CreateContract(objectType);

			if(Converters.TryGetValue(objectType, out JsonConverter c))
			{
				contract.Converter = c;
			}

			return contract;
		}

		private Dictionary<Type, string[]> IgnoredPropertie = new Dictionary<Type, string[]>()
		{
			{typeof(IPanel), new [] { nameof(IPanel.Children) }},
			{typeof(Page), new [] { nameof(Page.Content), nameof(Page.Navigation), }},
		};

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);
			var type = property.DeclaringType.GetTypeInfo();

			foreach (var item in IgnoredPropertie)
			{
				var pname = property.PropertyName;
				if (item.Key.GetTypeInfo().IsAssignableFrom(type) && item.Value.Contains(pname))
				{
					property.ShouldSerialize = (x) => false;
				}
			}

	        return property;
	    }
	}
}
