using System;
using System.Linq;
using System.IO;

namespace Mwm.UI.Xaml.Build
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var projectFolder = args.FirstOrDefault() ?? Directory.GetCurrentDirectory();
				var generator = new XamlGenerator();
				generator.Generate(projectFolder);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed: {ex}");
			}
		}
	}
}
