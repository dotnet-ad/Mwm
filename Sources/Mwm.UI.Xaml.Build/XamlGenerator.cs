namespace Mwm.UI.Xaml
{
	using System;
	using System.Text;
	using System.Xml.Linq;
	using System.Reflection;
	using System.Collections.Generic;
	using System.Linq;
	using System.IO;

	public class XamlGenerator
	{
		public class GeneratedResult
		{
			public GeneratedResult()
			{
				this.Usings.Add("Mwm.UI");
				this.Usings.Add("System.CodeDom.Compiler");
			}

			public string Class { get; set; }

			public Type BaseType { get; set; }

			public List<string> Fields { get; set; } = new List<string>();

			public List<string> Usings { get; set; } = new List<string>();

			public StringBuilder InitializeBody { get; set; } = new StringBuilder();

			public override string ToString()
			{
				var builder = new StringBuilder();

				if (string.IsNullOrEmpty(this.Class))
					throw new NullReferenceException("Class attribute must be defined");

				var classSplits = Class?.Trim().Split('.');
				var className = classSplits.Last();
				var classNamespace = string.Join(".", classSplits.Take(classSplits.Length - 1));

				foreach (var u in this.Usings)
				{
					builder.AppendLine($"using {u};");
				}

				builder.AppendLine();
				builder.AppendLine($"namespace {classNamespace}");
				builder.AppendLine("{");

				builder.AppendLine();
				builder.AppendLine("  [GeneratedCodeAttribute(\"Mwm\", \"1.0.0.0\")]");
				builder.AppendLine($"  public partial class {className} : {BaseType.FullName}");
				builder.AppendLine("  {");

				if (this.Fields.Count > 0)
				{
					builder.AppendLine();
					builder.AppendLine("    // Fields");
					foreach (var f in this.Fields)
					{
						builder.AppendLine(f);
					}
				}

				builder.AppendLine();
				builder.AppendLine("    protected void Initialize()");
				builder.AppendLine("    {");

				builder.AppendLine(this.InitializeBody.ToString());

				builder.AppendLine("    }");
				builder.AppendLine("  }");
				builder.AppendLine("}");

				return builder.ToString();
			}
		}

		public XamlGenerator()
		{
		}

		private int lastId = 0;

		protected static XNamespace XNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

		#region Attributes

		private string GenerateAttributeValue(Type type, XAttribute att)
		{
			var property = type.GetRuntimeProperty(att.Name.LocalName);

			if (property == null)
				return null;

			var value = att.Value;
			var ptype = property.PropertyType;

			if (ptype == typeof(string))
			{
				return $"\"{value}\"";
			}

			if (ptype == typeof(Thickness))
			{
				var t = Thickness.Parse(value);
				return $"new Thickness({t.Left},{t.Top},{t.Right},{t.Bottom})";
			}

			if (ptype == typeof(Color))
			{
				var t = Color.Parse(value);
				return $"new Color({t.R},{t.G},{t.B},{t.A})";
			}

			if (ptype == typeof(int) || ptype == typeof(double) || ptype == typeof(bool))
			{
				return $"{value.ToLower()}";
			}

			if (ptype.GetTypeInfo().IsEnum)
			{
				return $"{ptype.FullName}.{value}";
			}

			throw new InvalidOperationException($"Can't parse attribute of type {ptype}");
		}

		private string GenerateAttribute(string parentId, Type type, XAttribute att)
		{
			var value = GenerateAttributeValue(type, att);

			if (!string.IsNullOrEmpty(value))
			{
				return $"      {parentId}.{att.Name.LocalName} = {value};";
			}

			var e = type.GetRuntimeEvent(att.Name.LocalName);

			if (e != null)
			{
				return $"      {parentId}.{att.Name.LocalName} += {att.Value};";
			}

			return null;

		}

		#endregion

		#region Nodes

		private string GenerateNode(GeneratedResult result, XElement xml, bool isRoot)
		{
			var type = Type.GetType($"Mwm.UI.{xml.Name.LocalName}, Mwm.UI");

			string id;
			if (isRoot)
			{
				id = "this";
				result.Class = xml.Attribute(XNamespace + "Class")?.Value;
				result.BaseType = type;
			}
			else
			{
				// Constructor
				id = $"v_{lastId++}";
				result.InitializeBody.AppendLine($"      var {id} = new {type.FullName}();");
			}

			// Attributes
			foreach (var attribute in xml.Attributes())
			{
				var lines = GenerateAttribute(id, type, attribute);
				if (lines != null)
					result.InitializeBody.AppendLine(lines);
			}

			// Panel's children
			if (typeof(IPanel).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
			{
				foreach (var child in xml.Elements())
				{
					var childid = GenerateNode(result, child, false);
					result.InitializeBody.AppendLine($"      {id}.AddChild({childid});");
				}
			}
			else if (typeof(Page).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
			{
				var childid = GenerateNode(result, xml.Elements().First(), false);
				result.InitializeBody.AppendLine($"      {id}.Content = {childid};");
			}

			// Field
			var fieldName = xml.Attribute(XNamespace + "Name")?.Value;
			if (fieldName != null && !isRoot)
			{
				result.InitializeBody.AppendLine($"      this.{fieldName} = {id};");
				result.Fields.Add($"protected {type.FullName} {fieldName};");
			}

			return id;
		}

		#endregion

		#region Generation

		private string GenerateContent(string xaml)
		{
			var xml = XDocument.Parse(xaml);
			var result = new GeneratedResult();
			this.GenerateNode(result, xml.Root, true);
			return result.ToString();
		}

		private void GenerateFile(string inputFile, string outputFile = null)
		{
			outputFile = outputFile ?? inputFile.Replace(".xaml", ".xaml.g.cs");

			Console.WriteLine($"Generating {inputFile} -> {outputFile} ...");

			var xaml = File.ReadAllText(inputFile);

			if (File.Exists(outputFile))
				File.Delete(outputFile);

			var parentdir = Directory.GetParent(outputFile);
			if (!parentdir.Exists)
			{
				parentdir.Create();
			}

			var csharp = GenerateContent(xaml);

			File.WriteAllText(outputFile, csharp);
		}

		private void GenerateFolder(string inputFolder, string outputFolder = null)
		{
			outputFolder = outputFolder ?? Path.Combine(inputFolder, "obj");

			var files = Directory.GetFiles(inputFolder, "*.xaml", SearchOption.AllDirectories);
			foreach (var xamlFile in files)
			{
				var xamlgFile = Path.Combine(outputFolder, xamlFile.Replace(inputFolder, "").TrimStart('/', '\\')).Replace(".xaml", ".xaml.g.cs");
		
				this.GenerateFile(xamlFile,xamlgFile);
			}
		}

		public void Generate(string input, string output = null)
		{
			if (Directory.Exists(input))
			{
				this.GenerateFolder(input, output);
			}
			else
			{
				this.GenerateFile(input, output);
			}
		}

		#endregion
	}
}
