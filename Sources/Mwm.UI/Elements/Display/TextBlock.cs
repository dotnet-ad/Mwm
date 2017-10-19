namespace Mwm.UI
{
	public class TextBlock : Element
	{
		public TextBlock()
		{
			this.fontSize = 12;
		}

		private string text;

		private int fontSize;

		private Color foreground;

		public Color Foreground
		{
			get => this.foreground;
			set => this.Set(ref foreground, value);
		}

		public string Text
		{
			get => this.text;
			set => this.Set(ref text, value);
		}

		public int FontSize
		{
			get => this.fontSize;
			set => this.Set(ref fontSize, value);
		}
	}
}
