namespace Mwm.UI
{
	using System;

	public class Slider : Control
	{
		public class ValueChangedEventArgs
		{
			public double NewValue { get; set; }
		}

		public Slider()
		{
			this.ValueChanged += (sender, e) => this.Value = e.NewValue;
		}

		private double value;

		private Color foreground;

		public Color Foreground
		{
			get => this.foreground;
			set => this.Set(ref foreground, value);
		}

		public double Value
		{
			get => this.value;
			set => this.Set(ref value, value);
		}

		public event EventHandler<ValueChangedEventArgs> ValueChanged;

		protected void RaiseValueChanged(double value) { this.ValueChanged?.Invoke(this, new ValueChangedEventArgs { NewValue = value }); }
	}
}
