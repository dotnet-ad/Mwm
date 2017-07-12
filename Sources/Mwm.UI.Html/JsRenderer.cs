namespace Mwm.UI.Html
{
	using Newtonsoft.Json;
	using System.Text;
	using System.Reflection;

	public class JsRenderer : IRenderer
	{
		public string Render(IElement element)
		{
			var resolver = new ElementContractResolver();

			var jsonsettings = new JsonSerializerSettings()
			{
				ContractResolver = resolver,
			};

			var values = JsonConvert.SerializeObject(element,jsonsettings);

			var typeName = typeof(Page).GetTypeInfo().IsAssignableFrom(element.GetType().GetTypeInfo()) ? "Page" : element.GetType().Name;

			var js = new StringBuilder();
			js.Append($"new {typeName}(");
			js.Append(values);
			js.Append($")");

			if(element is IPanel panel)
			{
				foreach (var child in panel.Children)
				{
					js.Append($".add({ this.Render(child) })");
				}
			}
			else if (element is Page page)
			{
				js.Append($".content({ this.Render(page.Content) })");

			}

			return js.ToString();
		}
	}
}
