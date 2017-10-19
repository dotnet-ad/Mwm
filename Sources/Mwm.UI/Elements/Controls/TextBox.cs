namespace Mwm.UI
{
	using System;

	public class TextBox : Control
	{
		public class TextChangedEventArgs
		{
			public string NewText { get; set; }
		}

		public TextBox()
		{
			this.fontSize = 12;
			this.TextChanged += (sender, e) => this.Text = e.NewText;
		}

		private string text, placeholderText;

		private int fontSize;

		private Color foreground, borderColor;

		private Thickness borderThickness = new Thickness(0, 0, 0, 0);

		public Color Foreground
		{
			get => this.foreground;
			set => this.Set(ref foreground, value);
		}

		public Color BorderColor
		{
			get => this.borderColor;
			set => this.Set(ref borderColor, value);
		}

		public Thickness BorderThickness
		{
			get => this.borderThickness;
			set => this.Set(ref borderThickness, value);
		}

		public string Text
		{
			get => this.text;
			set => this.Set(ref text, value);
		}

		public string PlaceholderText
		{
			get => this.placeholderText;
			set => this.Set(ref placeholderText, value);
		}

		public int FontSize
		{
			get => this.fontSize;
			set => this.Set(ref fontSize, value);
		}

		public event EventHandler<TextChangedEventArgs> TextChanged;

		protected void RaiseTextChanged(string text) { this.TextChanged?.Invoke(this, new TextChangedEventArgs { NewText = text }); }
	}
}
