namespace Snapdragon.Effects
{
    public record SwitchCardSide(ICard Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            if (actualCard == null)
            {
                return game;
            }

            var location = game[actualCard.Column];

            // TODO: Handle other slot limits
            if (location[actualCard.Side.Other()].Count >= Max.CardsPerLocation)
            {
                return game;
            }

            return game.SwitchCardSideUnsafe(actualCard);
        }
    }
}
