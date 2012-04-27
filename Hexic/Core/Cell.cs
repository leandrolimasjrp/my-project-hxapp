namespace Hexic.Core
{
	public class Cell
	{
		public Cell(int _x, int _y)
		{
			X = _x;
			Y = _y;
		}

		public int X { get; private set; }

		public int Y { get; private set; }

		public int Value { get; set; }

		public bool IsEmpty
		{
			get { return Value == 0; }
		}

		public void Clear()
		{
			Value = 0;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1}]", X, Y);
		}
	}
}