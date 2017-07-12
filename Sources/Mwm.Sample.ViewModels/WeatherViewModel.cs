namespace Mwm.Sample.ViewModels
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Linq;

	public class WeatherViewModel : ViewModelBase
	{
		public class ForecastItemViewModel
		{
			public string Time { get; set; }

			public double MinTemperature { get; set; }

			public double MaxTemperature { get; set; }

			public string Icon { get; set; }
		}

		public WeatherViewModel(IWeatherApi api)
		{
			this.api = api;
			this.UpdateCommand = new AsyncRelayCommand(ExecuteUpdateCommand);
		}

		#region Fields

		private string name;

		private IWeatherApi api;

		private bool isUpdating;

		#endregion

		private IEnumerable<ForecastItemViewModel> locations;

		public bool IsUpdating
		{
			get => this.isUpdating;
			set => this.Set(ref this.isUpdating, value);
		}

		public string Name
		{
			get => this.name;
			set => this.Set(ref this.name, value);
		}

		public IEnumerable<ForecastItemViewModel> Forecasts
		{
			get => this.locations;
			set => this.Set(ref this.locations, value);
		}

		public AsyncRelayCommand UpdateCommand { get; }

		public static DateTime FromTimestamp(long unixTimeStamp)
		{
			var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dt = dt.AddSeconds(unixTimeStamp).ToLocalTime();
			return dt;
		}

		private async Task ExecuteUpdateCommand(CancellationToken token)
		{
			try
			{
				this.IsUpdating = true;
				var city = await this.api.GetForecast();
				this.Name = city?.city?.name;
				this.Forecasts = city.list.Select(x => new ForecastItemViewModel
				{
					MinTemperature = x.main.temp_min,
					MaxTemperature = x.main.temp_max,
					Time = FromTimestamp(x.dt).ToString(),
					Icon = this.api.GetIcon(x.weather.FirstOrDefault()),
				});
			}
			catch (Exception ex)
			{

			}
			finally
			{
				this.IsUpdating = false;
			}
		}
	}
}
