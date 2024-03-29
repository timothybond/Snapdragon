namespace Snapdragon.Effects
{
    public record AddCopyToLocation(ICardInstance Card, Column Column, Side? Side = null) : IEffect
    {
        public Game Apply(Game game)
        {
            // TODO: Handle anything that restricts the slots
            if (game[Column][Card.Side].Count >= Max.CardsPerLocation)
            {
                return game;
            }

            var card = game.GetCard(Card.Id);

            if (card == null)
            {
                return game;
            }

            return game.WithCopyInPlay(card, Column, Side ?? Card.Side);
        }
    }
}
