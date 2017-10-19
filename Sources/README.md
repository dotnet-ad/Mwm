# Mwm

Build web interfaces with XAML markup language and bind it to the same view models than your mobile or desktop applications.

## Disclaimer

**Mwm** is purely **experimental**. It's absolutely not thought to be production-ready, this is more a proof-of-concept of a naïve implementation on top of [ASP.NET Core](https://www.asp.net/core/overview/aspnet-vnext) and [Websockets](https://developer.mozilla.org/fr/docs/WebSockets).

Moreover it's only been tested with **Google Chrome - Version 59.0.3071.115 (64-bit)**, running on **macOS 10.12**.

## Quickstart

**Counter.xaml**

```xaml
```

**Counter.xaml.cs**

```csharp
```

**Startup.cs**

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
	loggerFactory.AddConsole(Configuration.GetSection("Logging"));
	loggerFactory.AddDebug();
	app.UseWebSockets();
	app.UseMwm(new MwmOptions
	{
		UIBuilder = new FactoryBuilder().Register<Counter>(),
	});
}
```

## Roadmap

- [X] Middleware
	- [X] Sending property changes
	- [X] Receiving events
	- [X] Navigation
		- [X] Forward
		- [ ] Backward & history 
- [X] XAML
	- [X] Generation 
- [ ] UI 
	- [ ] Bindings
		- [X] One way 
		- [X] Two way 
		- [ ] One time
		- [ ] Converters
	- [X] Displays
		- [X] TextBlock
		- [X] Image
		- [X] Rectangle
		- [X] ProgressRing
		- [ ] ProgressBar
		- [ ] Border
	- [ ] Controls
		- [X] TextBox
		- [X] ToggleSwitch
		- [ ] Slider
		- [ ] DatePicker
		- [ ] ComboBox
		- [ ] RadioButton
	- [ ] Panels
		- [X] StackPanel
		- [ ] Grid
		- [ ] ScrollView
	- [ ] Virtualization
		- [ ] ListView
	- [ ] Migrate to Typescript

## How it works

#### Synchronization

All the logic is executed by an ASP.NET Core middleware that sends and listen to commands from a web app. All property changes, user interaction events, navigation are propagated to the javascript client through WebSockets.

**Details** : [MwmMiddleware.cs](MwmMiddleware.cs), [ElementObserver.cs](ElementObserver.cs)

#### Components & XAML Generation

The components and the generation are very basic at the moment. Theorically, the same concept should be applyable to *Xamarin.Forms* but with a lot more work. :)

**Details** : [Mwm.UI](Mwm.UI), [Mwm.UI.Xaml.Build](Mwm.UI.Xaml.Build)

### Contributions

Contributions are welcome! If you find a bug please report it and if you want a feature please report it.

If you want to contribute code please file an issue and create a branch off of the current dev branch and file a pull request.

### License

MIT © [Aloïs Deniel](http://aloisdeniel.github.io)