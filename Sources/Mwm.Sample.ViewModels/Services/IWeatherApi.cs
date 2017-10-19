namespace Mwm.Sample.ViewModels
{
	using System;
	using System.Threading.Tasks;

	public interface IWeatherApi
	{
		Task<CityForecast> GetForecast();

		string GetIcon(Weather w);
	}
}
