using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hexic.Core;

namespace Hexic.Current
{
	class SimplestPlayer : IPlayer
	{
		public PlayersTurn GetNextTurn(Board _board)
		{
			var unwantedCells = _board.Triplets.GroupBy(_triplet => _triplet.UnwantedCell);

			var tryRotate = new List<Triplet>();
			foreach (var group in unwantedCells)
			{
				if (group.Key == null)
				{
					//not matched
					continue;
				}
				tryRotate.AddRange(group);
				tryRotate.AddRange(_board.Triplets.Where(_triplet => _triplet.Cells.Contains(group.Key)));
			}

			Debug.WriteLine(tryRotate.Distinct().Count());

			var maxPoints = 0;
			PlayersTurn result = null;
			foreach (var triplet in tryRotate.Distinct())
			{
				foreach (var clockwise in new[]{false,true})
				{
					BeginAnalyze(_board);
					triplet.Rotate(clockwise);
					var points = _board.FindAndRemoveMatchesTotal();
					if(points>maxPoints)
					{
						maxPoints = points;
						result = new PlayersTurn(triplet, clockwise);
					}
				}
			}
			
			EndAnalyze(_board);
			return result;
		}

		private static void BeginAnalyze(Board _board)
		{
			foreach (var cell in _board.Cells)
			{
				cell.BeginAnalyze();
			}
		}

		private static void EndAnalyze(Board _board)
		{
			foreach (var cell in _board.Cells)
			{
				cell.EndAnalize();
			}
		}

		public override string ToString()
		{
			return GetType().Name;
		}
	}
}