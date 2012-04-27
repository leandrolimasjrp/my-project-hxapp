using System;

namespace Hexic.Current
{
	class HexValuesGenerator:IHexValuesGenerator
	{
		//initialized by constant seed to help debug project
		private readonly Random m_rnd = new Random(1);

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