namespace Mwm.UI
{
	using System.Collections.Generic;

	public interface IPanel : IElement
	{
		IEnumerable<IElement> Children { get; }

		void AddChild(IElement child);
	}
}
