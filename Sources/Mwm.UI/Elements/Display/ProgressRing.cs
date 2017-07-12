namespace Mwm.UI
{
	public class ProgressRing : Element
	{
		public ProgressRing()
		{
			this.Foreground = Color.MsBlue;
		}

		private bool isActive;

		private Color foreground;

		public Color Foreground
		{
			get => this.foreground;
			set => this.Set(ref foreground, value);
		}

		public bool IsActive
		{
			get => this.isActive;
			set => this.Set(ref isActive, value);
		}
	}
}
