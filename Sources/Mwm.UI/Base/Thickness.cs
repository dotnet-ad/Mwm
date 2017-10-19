namespace Mwm.UI
{
	using System.Linq;

	public class Thickness
	{
		public Thickness(int value) : this(value, value)
		{
		}

		public Thickness(int horizontal, int vertical) : this(horizontal, vertical,horizontal, vertical)
		{
		}

		public Thickness(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Right = right;
			this.Top = top;
			this.Bottom = bottom;
		}

		public int Left { get; }

		public int Right { get; }

		public int Top { get; }

		public int Bottom { get; }

		public static Thickness Parse(string value)
		{
			if (string.IsNullOrEmpty(value.Trim()))
				return new Thickness(0, 0, 0, 0);

			var splits = value.Split(',').Select(x => x.Trim()).ToArray();

			int left = 0, top = 0, right = 0, bottom = 0;

			if(splits.Length == 1)
			{
				left = int.Parse(splits[0]);
				right = left;
				top = left;
				bottom = left;
			}
			else if (splits.Length < 4)
			{
				left = int.Parse(splits[0]);
				right = left;
				top = int.Parse(splits[1]);
				bottom = top;
			}
			else
			{
				left = int.Parse(splits[0]);
				top = int.Parse(splits[1]);
				right = int.Parse(splits[2]);
				bottom = int.Parse(splits[3]);
			}

			return new Thickness(left, top, right, bottom);
		}
	}
}
