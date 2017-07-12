namespace Mwm.AspNetCore
{
	using Mwm.UI;
	using Mwm.UI.Html;

	public class MwmOptions
	{
		public MwmOptions()
		{
			this.Renderer = new JsRenderer();
			this.Serializer = new ValueSerializer();
		}

		public ISerializer Serializer { get; set; }

		public IRenderer Renderer { get; set; }

		public IUIBuilder UIBuilder { get; set; }
	}
}
