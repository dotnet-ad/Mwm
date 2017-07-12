using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Mwm.Sample.ViewModels
{
	public class HomeViewModel : ViewModelBase
	{
		public HomeViewModel()
		{
			this.Title = "Not updated";
			this.UpdateCommand = new AsyncRelayCommand(ExecuteUpdateCommand);
		}

		private string title;

		private bool isUpdating;

		public string Title
		{
			get => this.title;
			set => this.Set(ref this.title, value);
		}

		public bool IsUpdating
		{
			get => this.isUpdating;
			set => this.Set(ref this.isUpdating, value);
		}

		public AsyncRelayCommand UpdateCommand { get; }

		private async Task ExecuteUpdateCommand(CancellationToken token)
		{
			this.IsUpdating = true;
			this.Title = "updating";
			await Task.Delay(1000);
			this.Title = "updating.";
			await Task.Delay(1000);
			this.Title = "updating..";
			await Task.Delay(1000);
			this.Title = "updating...";
			await Task.Delay(4000);
			this.Title = "updated!";
			this.IsUpdating = false;
		}
	}
}
