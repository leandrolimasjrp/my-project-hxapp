using System.Collections.Generic;
using System.Linq;

namespace Hexic.Core
{
	public class Triplet
	{
		public int MatchColor
		{
			get
			{
				if (Cells[0].Value == Cells[1].Value && Cells[0].Value == Cells[2].Value)
				{
					return Cells[0].Value;
				}
				return 0;
			}
		}

		/// <summary>
		/// Cells shoud be in clockwise order
		/// </summary>
		public Triplet(IEnumerable<Cell> _cells)
		{
			Cells = _cells.ToArray();
		}

		public Cell[] Cells { get; private set; }

		public int Index { get; set; }

		public int Rotate()
		{
			var zval = Cells[0].Value;
			Cells[0].Value = Cells[2].Value;
			Cells[2].Value = Cells[1].Value;
			Cells[1].Value = zval;
			return 0;
		}

		public override string ToString()
		{
			return string.Format("T:{0}\t{1}", Index, string.Join(",", Cells.Select(_cell => _cell.ToString())));
		}
	}
}