using System;
using System.Threading.Tasks;

namespace Mwm.UI
{
	public interface INavigation
	{
		Task NavigateAsync(string page);
	}
}
