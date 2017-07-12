namespace Mwm.Sample
{
	using System;

	public partial class Counter
	{
		public Counter()
		{
			this.Initialize();
		}

		private int count;

		private void Add(object sender, EventArgs args)
		{
			this.title.Text = $"Count: {++count}";
		}
	}
}
