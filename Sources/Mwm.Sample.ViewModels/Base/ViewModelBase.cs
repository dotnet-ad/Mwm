using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Mwm.Sample.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		#region Observable

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool Set<T>(ref T field, T value, [CallerMemberName]string name = null)
		{
			var changed = !EqualityComparer<T>.Default.Equals(field, value);

			if (changed)
			{
				field = value;
				RaiseProperty(name);
			}

			return changed;
		}

		public void RaiseProperty(string property) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

		public void RaiseProperties(params string[] properties)
		{
			foreach (var property in properties)
			{
				this.RaiseProperty(property);
			}
		}

		#endregion
	}
}
