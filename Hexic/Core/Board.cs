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
			var result = new List<Triplet>();
			Action<Tuple<int, int>[]> checkAndAdd = delegate(Tuple<int, int>[] _tuples)
			                                	{
													if (_tuples.All(_tuple => IsCellValid(_tuple.Item1, _tuple.Item2)))
													{
														result.Add(new Triplet(_tuples.Select(_tuple => Cells[_tuple.Item1, _tuple.Item2])));
													}
			                                	};

			if ((_row & 1) == 1)
			{
				checkAndAdd(new[] { Tuple.Create(_col, _row), Tuple.Create(_col + ((_row ^ 1) & 1), _row - 1), Tuple.Create(_col + ((_row ^ 1) & 1), _row + 1) });
				checkAndAdd(new[] { Tuple.Create(_col, _row), Tuple.Create(_col, _row + 2), Tuple.Create(_col + ((_row ^ 1) & 1), _row + 1) });
			}
			else
			{
				checkAndAdd(new[] { Tuple.Create(_col, _row), Tuple.Create(_col + ((_row ^ 1) & 1), _row - 1), Tuple.Create(_col + ((_row ^ 1) & 1), _row + 1) });
				checkAndAdd(new[] { Tuple.Create(_col, _row), Tuple.Create(_col + 1, _row + 1), Tuple.Create(_col, _row + 2) });
			}

			return result;
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

		public int FindAndRemoveMatches(bool _printDebugInfo)
		{
			var matches = Triplets.GroupBy(_triplet => _triplet.Match3Color);
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

				var matched = group.Select(_triplet => _triplet.Index).ToList();
				while(matched.Count>0)
				{
					united.Clear();
					
					united.Add(matched[0]);

					toCheck.Add(matched[0]);
					do
					{
						toAdd.Clear();
						foreach (var i in toCheck)
						{
							toAdd.AddRange(matched.Where(_j => !united.Contains(_j) && m_tripletsNeighbours[i, _j]));
						}
						united.AddRange(toAdd);
						toCheck.Clear();
						toCheck.AddRange(toAdd);
					} while (toAdd.Count > 0);

					var pow = (int)Math.Pow(3, united.Count);
					if (_printDebugInfo)
					{
						foreach (var i in united)
						{
							Debug.WriteLine(Triplets[i]);
						}

						Debug.WriteLine("* matches " + united.Count + " triplets, v=" + Triplets[united[0]].Cells[0].Value + ", gained " + pow + " points.");
					}

					points += pow;

					foreach (var i in united)
					{
						foreach (var cell in Triplets[i].Cells)
						{
							cell.Clear();
						}
						matched.Remove(i);
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

		public int FindAndRemoveMatchesTotal()
		{
			var points = 0;
			var phase = Game.ETurnPhase.CHECK_MATCHES;
			do
			{
				switch (phase)
				{
					case Game.ETurnPhase.FALL_DOWN:
						FallCells();
						phase = Game.ETurnPhase.CHECK_MATCHES;
						break;
					case Game.ETurnPhase.CHECK_MATCHES:
						var toAdd = FindAndRemoveMatches(false);
						if(toAdd>0)
						{
							points += toAdd;
							phase = Game.ETurnPhase.FALL_DOWN;
						}
						else
						{
							return points;
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} while (true);
		}
	}
}
