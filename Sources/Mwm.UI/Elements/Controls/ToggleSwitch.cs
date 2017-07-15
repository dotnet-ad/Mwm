namespace Mwm.UI
{
	using System;

	public class ToggleSwitch : Control
	{
		public class IsOnChangedEventArgs
		{
			public bool NewIsOn { get; set; }
		}

		public ToggleSwitch()
		{
			this.IsOnChanged += (sender, e) => this.IsOn = e.NewIsOn;
		}

		private bool isOn;

		private Color foreground;

		public Color Foreground
		{
			get => this.foreground;
			set => this.Set(ref foreground, value);
		}

		public bool IsOn
		{
			get => this.isOn;
			set => this.Set(ref isOn, value);
		}

		public event EventHandler<IsOnChangedEventArgs> IsOnChanged;

		protected void RaiseIsOnChanged(bool isOn) { this.IsOnChanged?.Invoke(this, new IsOnChangedEventArgs { NewIsOn = isOn }); }
	}
}
