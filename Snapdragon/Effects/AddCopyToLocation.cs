namespace Snapdragon.Effects
{
    public record AddCopyToLocation(ICard Card, Column Location) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Handle anything that restricts the slots
            if (game[Location][Card.Side].Count >= 4)
            {
                return game;
            }

            var card = Card.InPlayAt(Location) with
            {
                State = CardState.InPlay,
                Id = Ids.GetNext<CardInstance>()
            };

            var location = game[Location].WithCard(card);
            return game.WithLocation(location);
        }
    }
}
