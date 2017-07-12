namespace Mwm.UI
{
	public class Image : Element
	{
		private string source;

		public string Source
		{
			get => this.source;
			set => this.Set(ref source, value);
		}
	}
}
