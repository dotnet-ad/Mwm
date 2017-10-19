namespace Mwm.UI
{
	using System;

	public class Page : Element
	{
		public Page()
		{
		}

		public INavigation Navigation { get; set; }

		public IElement Content { get; set; }
	}
}
