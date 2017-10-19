using Mwm.Sample.ViewModels;
namespace Mwm.Sample
{
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using Mwm.AspNetCore;
	using Mwm.UI;

	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();
			app.UseWebSockets();
			app.UseMwm(new MwmOptions
			{
				UIBuilder = new FactoryBuilder().Register<Home>()
												.Register<Controls>()
				                                .Register<Counter>()
												//.Register<Weather>()
												//.Register("Counter", CreateCounter),
			});
		}

		private IElement CreateCounter()
		{
			int count = 0;
			var result = new StackPanel() { Margin = new Thickness(10) };
			var label = new TextBlock { Text = $"Count: {count}", Margin = new Thickness(10), };
			var button = new Button { Text = "+1", Margin = new Thickness(10), };
			button.Click += (sender, e) => label.Text = $"Count: {++count}";
			result.AddChild(label);
			result.AddChild(button);
			return result;
		}
	}
}
