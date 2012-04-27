using System;
using System.Text;
using Hexic.Core;

namespace Hexic.Current
{
	class ConsoleBwRenderer : IRenderer
	{
		private readonly Game m_game;
		private readonly string[] m_chars = new[] { " ", new string((char)1, 1), new string((char)2, 1), "*", "@", "#" };

		private string m_prevState = String.Empty;

		public ConsoleBwRenderer(Game _game)
		{
			m_game = _game;
		}

		public void Render()
		{
			Console.Clear();

			Console.WriteLine("Previous STATE");
			Console.WriteLine(m_prevState);

			Console.WriteLine(Environment.NewLine +  "Current STATE");

			var sb = new StringBuilder();

			sb.AppendLine(string.Format("Score: {0:0000000}\t\t Turn:{1:00000000}", m_game.Score, m_game.Turn));
			sb.AppendLine(@"   _   _   _   _   _");
			
			for (var row = 0; row < Board.TOTAL_ROWS; ++row)
			{
				var odd = (row & 1) == 1;
				sb.Append(row == 0 ? @" _" : odd ? @"" : @"\_");
				sb.Append("/");
				for (var col = 0; col < Board.TOTAL_COLS; ++col)
				{
					sb.Append(m_chars[m_game.Board.Cells[col, row].Value]);
					if (col < (Board.TOTAL_COLS-1) || odd)
					{
						sb.Append(@"\_/");
					}
				}
				if (!odd)
				{
					sb.Append(@"\");
				}
				sb.AppendLine();
			}
			sb.AppendLine(@"  \_/ \_/ \_/ \_/ \_/");
			sb.AppendLine();

			m_prevState = sb.ToString();

			Console.WriteLine(m_prevState);
			Console.WriteLine("Press 'a' to exit form turn-by-turn mode.");
		}
	}
}