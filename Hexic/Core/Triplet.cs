using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexic.Core
{
	public class Triplet
	{
		/// <summary>
		/// Cells shoud be in clockwise order
		/// </summary>
		public Triplet(IEnumerable<Cell> _cells)
		{
			Cells = _cells.ToArray();
		}

		public int Match3Color
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


		public Cell UnwantedCell
		{
			get
			{
				if (Cells[0].Value == Cells[1].Value)
				{
					return Cells[2];
				}
				if (Cells[0].Value == Cells[2].Value)
				{
					return Cells[1];
				} 
				if (Cells[1].Value == Cells[2].Value)
				{
					return Cells[0];
				}
				return null;
			}
		}

		public Cell[] Cells { get; private set; }

		public int Index { get; set; }

		public int Rotate(bool _clockwise)
		{
			var zval = Cells[0].Value;
			if (_clockwise)
			{
				Cells[0].Value = Cells[2].Value;
				Cells[2].Value = Cells[1].Value;
				Cells[1].Value = zval;
			}
			else
			{
				Cells[0].Value = Cells[1].Value;
				Cells[1].Value = Cells[2].Value;
				Cells[2].Value = zval;
			}
			return 0;
		}

		public override string ToString()
		{
			return string.Format("T:{0}\t{1}", Index, string.Join(",", Cells.Select(_cell => _cell.ToString())));
		}

		public bool Contains(int _value)
		{
			return Cells.Any(_cell => _cell.Value == _value);
		}
	}
}