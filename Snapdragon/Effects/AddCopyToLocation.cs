using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record AddCopyToLocation(ICard Card, Column Column, Side? Side = null) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Handle anything that restricts the slots
            if (game[Column][Card.Side].Count >= Max.CardsPerLocation)
            {
                return game;
            }

            // TODO: Get current state of card instead of using reference
            var card = Card.InPlayAt(Column) with
            {
                State = CardState.InPlay,
                Id = Ids.GetNext<ICard>(),
                Side = Side ?? Card.Side
            };

            var location = game[Column].WithCard(card);
            return game.WithLocation(location)
                .WithEvent(new CardAddedToLocationEvent(card, Column, game.Turn));
        }
    }
}
