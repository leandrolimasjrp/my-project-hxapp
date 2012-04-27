using System;

namespace Hexic.Current
{
	class HexValuesGenerator:IHexValuesGenerator
	{
		//initialize by constant seed to help debug project
		private readonly Random m_rnd = new Random();

		private readonly int m_numberOfVariants;

		public HexValuesGenerator(int _numberOfVariants)
		{
			if(_numberOfVariants<3 || _numberOfVariants>7)
			{
				throw new ArgumentOutOfRangeException("_numberOfVariants should be in [3;7] range.");
			}
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