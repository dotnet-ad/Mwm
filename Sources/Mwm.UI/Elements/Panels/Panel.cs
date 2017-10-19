namespace Mwm.UI
{
	using System;
	using System.Collections.Generic;

	public abstract class Panel : Element, IPanel
	{
		private List<IElement> children = new List<IElement>();

		public IEnumerable<IElement> Children => this.children;

		public override void UpdateBindings()
		{
			base.UpdateBindings();

			foreach (var child in this.children)
			{
				child.UpdateBindings();
			}
		}

		public void AddChild(IElement child)
		{
			if (child.Parent != null)
				throw new InvalidOperationException("child already has a parent");

			child.Parent = this;
			this.children.Add(child);
			this.UpdateBindings();
		}
	}
}
