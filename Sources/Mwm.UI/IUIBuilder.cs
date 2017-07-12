namespace Mwm.UI
{
	public interface IUIBuilder
	{
		IElement Create(string name);

		bool CanCreate(string name);
	}
}
