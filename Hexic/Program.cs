using System;
using Hexic.Core;
using Hexic.Current;

namespace Hexic
{
    class Program
    {
        static void Main()
        {
			var player = new Player();
			var game = new Game(new HexValuesGenerator(3), player);
			var renderer = new ConsoleBwRenderer(game);
			//CheckAllTriplets(renderer, game);
			
			LetsPlay(renderer, game);
			Console.WriteLine(player + " gives up.");
			Console.ReadKey();
        }

		private static void LetsPlay(IRenderer _renderer, Game _game)
		{
			_renderer.Render();
			foreach (var gamePhase in _game.Turns())
    			{
					switch (gamePhase)
					{
						case Game.EGamePhase.JUST_REDRAW:
							_renderer.Render();
							break;
						case Game.EGamePhase.TURN_DONE:
							_renderer.Render();
							Console.ReadKey();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
    			Console.ReadKey();
    		}
			
		}

		private static void CheckAllTriplets(IRenderer _renderer, Game _game)
    	{
    		for (var i = 0; i < _game.Board.Triplets.Length; ++i)
    		{
				foreach (var cell in _game.Board.Cells)
				{
					cell.Clear();
				}

				_game.Board.Triplets[i].Cells[0].Value = 1;
				_game.Board.Triplets[i].Cells[1].Value = 2;
				_game.Board.Triplets[i].Cells[2].Value = 3;
				_renderer.Render();
				Console.ReadKey();
			}
		}
    }
}
