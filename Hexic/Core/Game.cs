using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hexic.Current;

namespace Hexic.Core
{
	class Game
	{
		public enum EGamePhase
		{
			JUST_REDRAW,
			TURN_DONE,
		}

		private enum ETurnPhase
		{
			FALL_DOWN,
			CHECK_MATCHES,
			FILL_EMPTY_CELLS,
			LETS_TURN,
			PLAYER_GIVES_UP,
		}

		private readonly Player m_player;

		public Board Board { get; private set; }

		public int Score { get; private set; }

		public int Turn { get; private set; }

		public Game(IHexValuesGenerator _hexValuesGenerator, Player _player)
		{
			m_player = _player;
			Board = new Board(_hexValuesGenerator);
		}

		public IEnumerable<EGamePhase> Turns()
		{
			var phase = ETurnPhase.FALL_DOWN;
			do
			{
				Debug.WriteLine(phase);
				switch (phase)
				{
					case ETurnPhase.FALL_DOWN:
						Board.FallCells();
						yield return EGamePhase.JUST_REDRAW;
						phase = ETurnPhase.CHECK_MATCHES;
						break;
					case ETurnPhase.CHECK_MATCHES:
						var passivePoints = Board.CheckMatches();
						if (passivePoints > 0)
						{
							Score += passivePoints;
							phase = ETurnPhase.FALL_DOWN;
							yield return EGamePhase.JUST_REDRAW;
						}
						else
						{
							phase = ETurnPhase.FILL_EMPTY_CELLS;
						}
						break;
					case ETurnPhase.FILL_EMPTY_CELLS:
						if(Board.FillEmptyCells())
						{
							yield return EGamePhase.JUST_REDRAW;
							phase = ETurnPhase.CHECK_MATCHES;
						}
						else
						{
							phase = ETurnPhase.LETS_TURN;
						}
						break;
					case ETurnPhase.LETS_TURN:
						var triplet = m_player.GetTripletToRotate(Board);
						var takenPoints = 0;
						if (triplet != null)
						{
							for (var i = 0; i < 3; ++i)
							{
								triplet.Rotate();
								takenPoints = Board.CheckMatches();
								if (takenPoints > 0)
								{
									break;
								}
							}
						}
						if(takenPoints>0)
						{
							Turn++;
							Score += takenPoints;
							phase = ETurnPhase.FALL_DOWN;
						}
						else
						{
							phase = ETurnPhase.PLAYER_GIVES_UP;
						}
						break;
					case ETurnPhase.PLAYER_GIVES_UP:
						yield break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			} while (phase != ETurnPhase.PLAYER_GIVES_UP);
		}
	}
}
