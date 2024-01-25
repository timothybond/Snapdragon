namespace Snapdragon
{
    public record PlayCardAction(Side Side, Card Card, Column Column) : IPlayerAction
    {
        public GameState Apply(GameState initialState)
        {

            throw new NotImplementedException();
        }
    }
}
