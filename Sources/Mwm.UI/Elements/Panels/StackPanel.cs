namespace Mwm.UI
{
	public class StackPanel : Panel
	{
		private Orientation orientation;

		public Orientation Orientation
		{
			get => this.orientation;
			set => this.Set(ref orientation, value);
		}
	}
}
