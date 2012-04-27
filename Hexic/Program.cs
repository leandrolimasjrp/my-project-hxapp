using System;
using Hexic.Core;
using Hexic.Current;

namespace Hexic
{
    class Program
    {
        static void Main()
        {
			var game = new Game(new HexValuesGenerator(3), new SimplestPlayer());
			var renderer = new ConsoleBwRenderer(game);

			//CheckAllGeneratedTriplets(renderer, game.Board);
			LetsPlay(renderer, game);

			Console.ReadKey(true);
        }

		private static void LetsPlay(IRenderer _renderer, Game _game)
		{
			_renderer.Render();
			var turnMode = true;
			foreach (var gamePhase in _game.Turns())
			{
				switch (gamePhase)
				{
					case Game.EGamePhase.BOARD_PREPARE:
						continue;
					case Game.EGamePhase.JUST_REDRAW:
						_renderer.Render();
						break;
					case Game.EGamePhase.TURN_DONE:
						_renderer.Render();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (turnMode)
				{
					turnMode = Console.ReadKey(true).Key != ConsoleKey.A;
				}
			}
			Console.WriteLine(_game.Player + " gives up.");
		}

		private static void CheckAllGeneratedTriplets(IRenderer _renderer, Board _board)
		{
			foreach (var triplet in _board.Triplets)
			{
				foreach (var cell in _board.Cells)
				{
					cell.Clear();
				}

				triplet.Cells[0].Value = 3;
				triplet.Cells[1].Value = 1;
				triplet.Cells[2].Value = 1;
				
				_renderer.Render();

				Console.WriteLine("Triplet: " + triplet);
				Console.ReadKey(true);
			}
		}
    }
}
