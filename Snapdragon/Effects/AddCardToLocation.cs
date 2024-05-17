namespace Snapdragon.Effects
{
    public record AddCardToLocation(CardDefinition Definition, Side Side, Column Column) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Handle restrictions on slots
            if (game[Column][Side].Count < Max.CardsPerLocation)
            {
                return game.WithNewCardInPlayUnsafe(Definition, Column, Side);
            }

            return game;
        }
    }
}
