namespace Mwm.UI
{
	public class Rectangle : Element
	{
		private Color background;

		public Color Background
		{
			get => this.background;
			set => this.Set(ref background, value);
		}
	}
}
