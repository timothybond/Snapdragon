namespace Snapdragon.Effects
{
    public record AddCardToLocation(CardDefinition Definition, Side Side, Column Column) : IEffect
    {
        public Game Apply(Game game)
        {
            var card = new Card(Definition, Side, CardState.InPlay) with { Column = Column };

            // TODO: Handle restrictions on slots
            if (game[Column][Side].Count < 4)
            {
                var location = game[Column];
                location = location.WithPlayedCard(card, Side);

                // TODO: Decide what event, if any, to raise for cards that were added but not played
                return game.WithLocation(location);
            }

            return game;
        }
    }
}
