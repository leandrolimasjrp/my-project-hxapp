namespace Hexic.Core
{
	public class PlayersTurn
	{
		public Triplet TripltetToTurn { get; private set; }
		public bool IsClockwise { get; private set; }

		public PlayersTurn(Triplet _tripltetToTurn, bool _isClockwise)
		{
			TripltetToTurn = _tripltetToTurn;
			IsClockwise = _isClockwise;
		}
	}
}
