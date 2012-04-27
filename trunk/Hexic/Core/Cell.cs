namespace Hexic.Core
{
	public class Cell
	{
		private int m_value;
		private int m_analizeValue;

		private bool m_analize = false;

		public Cell(int _x, int _y)
		{
			X = _x;
			Y = _y;
		}

		public int X { get; private set; }

		public int Y { get; private set; }

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
			return string.Format("[{0},{1}={2}]", X, Y, Value);
		}

		public void BeginAnalyze()
		{
			m_analize = true;
			m_analizeValue = m_value;
		}

		public void EndAnalize()
		{
			m_analize = false;
		}

		public int Value
		{
			get
			{
				return m_analize ? m_analizeValue : m_value;
			}
			set
			{
				if(m_analize)
				{
					m_analizeValue = value;
				}
				else
				{
					m_value = value;
				}
			}
		}
	}
}