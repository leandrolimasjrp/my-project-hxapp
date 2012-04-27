using Hexic.Core;

namespace Hexic
{
	public interface IPlayer
	{
		PlayersTurn GetNextTurn(Board _board);
	}
}
