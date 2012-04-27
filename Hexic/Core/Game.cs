using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hexic.Core
{
	class Game
	{
		public enum EGamePhase
		{
			BOARD_PREPARE,
			JUST_REDRAW,
			TURN_DONE,
		}

		public enum ETurnPhase
		{
			FALL_DOWN,
			CHECK_MATCHES,
			FILL_EMPTY_CELLS,
			LETS_TURN,
			PLAYER_GIVES_UP,
		}

		public Board Board { get; private set; }

		public int Score { get; private set; }

		public int Turn { get; private set; }

		public IPlayer Player { get; private set; }

		public Game(IHexValuesGenerator _hexValuesGenerator, IPlayer _player)
		{
			Player = _player;
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
						yield return Turn > 0 ? EGamePhase.JUST_REDRAW : EGamePhase.BOARD_PREPARE;
						phase = ETurnPhase.CHECK_MATCHES;
						break;
					case ETurnPhase.CHECK_MATCHES:
						var passivePoints = Board.FindAndRemoveMatches();
						if (passivePoints > 0)
						{
							if(Turn>0)
							{
								Score += passivePoints;
							}
							phase = ETurnPhase.FALL_DOWN;
							yield return Turn > 0 ? EGamePhase.JUST_REDRAW : EGamePhase.BOARD_PREPARE;
						}
						else
						{
							phase = ETurnPhase.FILL_EMPTY_CELLS;
						}
						break;
					case ETurnPhase.FILL_EMPTY_CELLS:
						if(Board.FillEmptyCells())
						{
							yield return Turn > 0 ? EGamePhase.JUST_REDRAW : EGamePhase.BOARD_PREPARE;
							phase = ETurnPhase.CHECK_MATCHES;
						}
						else
						{
							phase = ETurnPhase.LETS_TURN;
						}
						break;
					case ETurnPhase.LETS_TURN:
						if (Turn == 0) yield return EGamePhase.JUST_REDRAW;

						var turn = Player.GetNextTurn(Board);
						var takenPoints = 0;
						if (turn != null)
						{
							for (var i = 0; i < 3; ++i)
							{
								turn.TripltetToTurn.Rotate(turn.IsClockwise);
								takenPoints = Board.FindAndRemoveMatches();
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
