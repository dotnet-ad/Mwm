using System;
namespace Mwm.UI.Xaml.Build
{
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;

	public class MwmXamlTask : Task
	{
		[Required]
		public ITaskItem Source { get; set; }

		public string AssemblyName { get; set; }

		[Output]
		public string OutputFile { get; set; }

		public override bool Execute()
		{
			Log.LogMessage("Source: {0}", Source.ItemSpec);
			Log.LogMessage("AssemblyName: {0}", AssemblyName);
			Log.LogMessage("OutputFile {0}", OutputFile);

			try
			{
				var generator = new XamlGenerator();
				generator.Generate(Source.ItemSpec, OutputFile);
				return true;
			}
			catch (Exception e)
			{
				Log.LogMessage("Error: {0}", e.Message);
				Log.LogMessage("StackTrace: {0}", e.StackTrace);
				Log.LogError(null, null, null, Source, 0, 0, 0, 0, $"{e.Message}");
				return false;
			}
		}
	}
}
