namespace Mwm.UI
{
	public class Control : Element
	{
		public Control()
		{
			this.IsEnabled = true;
		}

		private bool isEnabled;

		public virtual bool IsEnabled
		{
			get => this.isEnabled;
			set => this.Set(ref isEnabled, value);
		}
	}
}
