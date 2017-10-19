namespace Mwm.UI
{
	using System.Collections.Generic;

	public class Grid : Panel
	{
		public static class Attached
		{
			public static int Row { get; } = 0;


			public static int Column { get; } = 0;

			public static int RowSpan { get; } = 1;

			public static int ColumnSpan { get; } = 1;
		}

		#region Column and row definitions

		public enum Unit
		{
			Auto,
			Pixel,
			Star,
		}

		public class RowDefinition
		{
			public RowDefinition(double height, Unit unit)
			{
				this.Height = height;
				this.Unit = unit;
			}

			public double Height { get; }

			public Unit Unit { get; }
		}


		public class ColumnDefinition
		{
			public ColumnDefinition(double width, Unit unit)
			{
				this.Width = width;
				this.Unit = unit;
			}

			public double Width { get; }

			public Unit Unit { get; }
		}

		#endregion

		public List<RowDefinition> RowDefinitions { get; } = new List<RowDefinition>();

		public List<ColumnDefinition> ColumnDefinitions { get; } = new List<ColumnDefinition>();
	}
}
