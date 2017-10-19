namespace Mwm.UI
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using System.Linq;

	public class Color
	{
		#region Constants

		public static readonly Color Black = new Color(0, 0, 0);
					   
		public static readonly Color White = new Color(255, 255, 255);
					   
		public static readonly Color Red = new Color(255, 0, 0);
					   
		public static readonly Color Green = new Color(0, 255, 0);
					   
		public static readonly Color Blue = new Color(0, 0, 255);

		public static readonly Color MsBlue = new Color(8, 166, 240);

		private static IDictionary<string, Color> knwownColors;

		private static bool TryParseNameColor(string name, out Color result)
		{
			if(knwownColors == null)
			{
				knwownColors = typeof(Color).GetRuntimeFields().Where(x => x.Attributes == FieldAttributes.Static).ToDictionary(x => x.Name.ToLowerInvariant(), x => (Color)x.GetValue(null));
			}

			return knwownColors.TryGetValue(name.Trim().ToLowerInvariant(), out result);
		}

		#endregion

		public Color(int r, int g, int b, int a = 255)
		{
			this.R = (byte)r;
			this.B = (byte)b;
			this.G = (byte)g;
			this.A = (byte)a;
		}

		public byte R { get; }

		public byte G { get; }

		public byte B { get; }

		public byte A { get; }

		public static Color Parse(string value)
		{
			int r = 0, g = 0, b = 0;

			// Named
			if(TryParseNameColor(value, out Color known))
			{
				return known;
			}

			// Hex
			if(value.StartsWith("#"))
			{
				value = value.TrimStart('#');

				if (value.Length == 1)
				{
					var cr = Char.ToString(value[0]);
					value = $"{cr}{cr}{cr}{cr}{cr}{cr}";
				}
				else if (value.Length == 3)
				{
					var cr = Char.ToString(value[0]);
					var cg = Char.ToString(value[1]);
					var cb = Char.ToString(value[2]);
					value = $"{cr}{cr}{cg}{cg}{cb}{cb}";
				}

				if (value.Length >= 6)
				{
					r = Convert.ToInt32(value.Substring(0, 2), 16);
					g = Convert.ToInt32(value.Substring(2, 2), 16);
					b = Convert.ToInt32(value.Substring(4, 2), 16);
				}
			}

			return new Color(r, g, b);
		}

		public string ToHex(bool withAlpha = false)
		{
			return "#" + this.R.ToString("X2") + this.G.ToString("X2") + this.B.ToString("X2") + (withAlpha ? this.A.ToString("X2") : "");
		}
	}
}
