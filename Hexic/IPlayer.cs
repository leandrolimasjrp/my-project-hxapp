using Hexic.Core;

namespace Hexic
{
	public interface IPlayer
	{
		Triplet GetTripletToRotate(Board _board);
	}
}
