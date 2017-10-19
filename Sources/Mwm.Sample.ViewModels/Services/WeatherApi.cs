namespace Mwm.Sample.ViewModels
{
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Newtonsoft.Json;

	public class WeatherApi : IWeatherApi
	{
		#region Url

		const string CityId = "6455259";

		const string AppId = "5def09a63acd859dfffa96ef43ba27a5";

		static readonly string Url = $"http://api.openweathermap.org/data/2.5/forecast?id={CityId}&APPID={AppId}";

		#endregion

		#region Fields

		private HttpClient client = new HttpClient();

 		#endregion

		public async Task<CityForecast> GetForecast()
		{
			var res = await client.GetAsync(Url);
			res.EnsureSuccessStatusCode();
			var json = await res.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<CityForecast>(json);
		}

		public string GetIcon(Weather w) => $"http://openweathermap.org/img/w/{w?.icon}.png";
	}
}
