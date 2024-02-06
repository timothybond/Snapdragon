namespace Snapdragon.Effects
{
    public record AddCopyToLocation(Card Card, Column Location) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Handle anything that restricts the slots
            if (game[Location][Card.Side].Count >= 4)
            {
                return game;
            }

            var card = Card with
            {
                State = CardState.InPlay,
                Id = Ids.GetNext<Card>(),
                Column = Location
            };

            var location = game[Location].WithCard(card);
            return game.WithLocation(location);
        }
    }
}
