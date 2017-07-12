using System;
namespace Mwm.Sample
{
	public partial class Home
	{
		public Home()
		{
			this.Initialize();
		}

		private void NavigateToControls(object sender, EventArgs args)
		{
			this.Navigation.NavigateAsync("Controls");
		}

		private void NavigateToCounter(object sender, EventArgs args)
		{
			this.Navigation.NavigateAsync("Counter");
		}
	}
}
