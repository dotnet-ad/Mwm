using System.Windows.Input;

namespace Mwm.UI
{
	using System;

	public class Button : Control
	{
		public Button()
		{
			this.Foreground = Color.White;
			this.Background = Color.MsBlue;
			this.Click += OnClick;
		}

		private string text;

		private ICommand command;

		private Color foreground, background, borderColor;

		private Thickness borderThickness = new Thickness(0,0,0,0);

		public override bool IsEnabled
		{
			get => base.IsEnabled && (this.Command?.CanExecute(null) ?? true);
			set => base.IsEnabled = value;
		}

		public Color Foreground
		{
			get => this.foreground;
			set => this.Set(ref foreground, value);
		}

		public Color Background
		{
			get => this.background;
			set => this.Set(ref background, value);
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

		public ICommand Command
		{
			get => this.command;
			set 
			{
				var old = this.command;
				if(this.Set(ref command, value))
				{
					if(old != null)
						old.CanExecuteChanged -= OnCommandExecuteChanged;
					
					this.RaiseProperty(nameof(this.IsEnabled));

					if(value != null)
						value.CanExecuteChanged += OnCommandExecuteChanged;
				}
			}
		}

		private void OnClick(object sender, EventArgs e)
		{
			try
			{
				if (this.IsEnabled)
					this.Command?.Execute(null);
			}
			catch (Exception ex)
			{

			}
		}

		private void OnCommandExecuteChanged(object sender, EventArgs e)
		{
			this.RaiseProperty(nameof(this.IsEnabled));
		}

		public event EventHandler Click;

		protected void RaiseClick() { this.Click?.Invoke(this, EventArgs.Empty); }
	}
}
