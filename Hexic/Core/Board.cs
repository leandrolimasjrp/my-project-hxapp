using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hexic.Core
{
	public class Board
	{
		public const int TOTAL_ROWS = 17;
		public const int TOTAL_COLS = 5;

		private bool[,] m_tripletsNeighbours;

		private readonly IHexValuesGenerator m_valuesGenerator;

		public Board(IHexValuesGenerator _valuesGenerator)
		{
			Cells = new Cell[TOTAL_COLS, TOTAL_ROWS];
			m_valuesGenerator = _valuesGenerator;

			for (var row = 0; row < TOTAL_ROWS; ++row)
			{
				for (var col = 0; col < TOTAL_COLS; ++col)
				{
					Cells[col, row] = new Cell(col, row);
				}
			}

			GenerateTriplets();

			FillEmptyCells();
		}

		public Cell[,] Cells { get; private set; }

		public Triplet[] Triplets { get; private set; }

		#region initialization

		private void GenerateTriplets()
		{
			var triplets = new List<Triplet>();
			var count = 0;
			for (var row = 0; row < TOTAL_ROWS; ++row)
			{
				for (var col = 0; col < TOTAL_COLS; ++col)
				{
					foreach (var triplet in GetCellTriplets(col, row))
					{
						triplet.Index = count++;
						triplets.Add(triplet);
					}
				}
			}

			Triplets = triplets.ToArray();

			//prepare neighbours array (triplets with common cells)
			m_tripletsNeighbours = new bool[count, count];
			for (var i = 0; i < count; ++i)
			{
				for (var j = i+1; j < count; ++j)
				{
					if (Triplets[i].Cells.Any(_cell => Triplets[j].Cells.Contains(_cell)))
					{
						m_tripletsNeighbours[i, j] = true;
						m_tripletsNeighbours[j, i] = true;
					}
				}
			}
		}

		private IEnumerable<Triplet> GetCellTriplets(int _col, int _row)
		{
			var neighbours = new[]
			                 	{

			                 		Tuple.Create(_col - (_row & 1), _row - 1),
			                 		Tuple.Create(_col, _row - 2),
			                 		Tuple.Create(_col + ((_row ^ 1) & 1), _row - 1),
			                 		Tuple.Create(_col + ((_row ^ 1) & 1), _row + 1),
			                 		Tuple.Create(_col, _row + 2),
			                 		Tuple.Create(_col - (_row & 1), _row + 1),
			                 	};

			var center = Tuple.Create(_col, _row);
			for (var i = 0; i < 6; ++i)
			{
				var tuples = new[] { neighbours[i], neighbours[(i + 1) % 6], center };
				if (tuples.All(_tuple => IsCellValid(_tuple.Item1, _tuple.Item2)))
				{
					yield return new Triplet(tuples.Select(_tuple => Cells[_tuple.Item1, _tuple.Item2]));
				}
			}
		}

		internal bool IsCellValid(int _x, int _y)
		{
			return _x >= 0 && _x < TOTAL_COLS && _y >= 0 && _y < TOTAL_ROWS;
		}

		#endregion

		public bool FillEmptyCells()
		{
			var result = false;
			for (var row = TOTAL_ROWS - 1; row >= 0; row--)
			{
				for (var col = 0; col < TOTAL_COLS; ++col)
				{
					if (Cells[col, row].IsEmpty)
					{
						Cells[col, row].Value = m_valuesGenerator.NextValue;
						result = true;
					}
				}
			}
			return result;
		}

		public int CheckMatches()
		{
			var matches = Triplets.GroupBy(_triplet => _triplet.MatchColor);
			var united = new List<int>();
			var toCheck = new List<int>();
			var toAdd = new List<int>();
			var points = 0;
			foreach (var group in matches)
			{
				if (group.Key == 0)
				{
					//not matched
					continue;
				}

				var triplets = group.Select(_triplet => _triplet.Index).ToList();
				while(triplets.Count>0)
				{
					united.Clear();
					
					united.Add(triplets[0]);

					

					toCheck.Add(triplets[0]);
					do
					{
						toAdd.Clear();
						foreach (var i in toCheck)
						{
							toAdd.AddRange(triplets.Where(_j => united.Contains(_j) && m_tripletsNeighbours[i, _j]));
						}
						united.AddRange(toAdd);
						toCheck.Clear();
						toCheck.AddRange(toAdd);
					} while (toAdd.Count > 0);

					var pow = (int) Math.Pow(3, united.Count);
					Debug.WriteLine("* matches " + united.Count + " triplets, v=" + Triplets[united[0]].Cells[0].Value + ", gained " + pow + " points.");

					points += pow;

					foreach (var i in united)
					{
						foreach (var cell in Triplets[i].Cells)
						{
							cell.Clear();
						}
						triplets.Remove(i);
					}
				}
			}
			return points;
		}

		public void FallCells()
		{
			bool flag;
			do
			{
				flag = false;
				for (var row = TOTAL_ROWS - 1; row > 1; row--)
				{
					for (var col = 0; col < TOTAL_COLS; ++col)
					{
						if (!Cells[col, row].IsEmpty) continue;
						if (Cells[col, row - 2].IsEmpty) continue;
						Cells[col, row].Value = Cells[col, row - 2].Value;
						Cells[col, row - 2].Value = 0;
						flag = true;
					}
				}
			} while (flag);
		}
	}
}
