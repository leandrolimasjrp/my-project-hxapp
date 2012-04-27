using System;

namespace Hexic.Current
{
	class HexValuesGenerator:IHexValuesGenerator
	{
		private readonly Random m_rnd = new Random();
		private readonly int m_numberOfVariants;

		public HexValuesGenerator(int _numberOfVariants)
		{
			m_numberOfVariants = _numberOfVariants;
		}

		public int NextValue
		{
			get
			{
				return m_rnd.Next(m_numberOfVariants) + 1;
			}
		}
	}
}