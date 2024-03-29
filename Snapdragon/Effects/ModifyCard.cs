namespace Snapdragon.Effects
{
    public abstract record ModifyCard(ICardInstance Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var card = game.Kernel[Card.Id];

            if (card == null)
            {
                return game;
            }

            if (IsBlocked(card, game))
            {
                return game;
            }

            var updatedCard = this.ApplyToCard(card.Base, game);
            return game.WithUpdatedCard(updatedCard);
        }

        protected abstract bool IsBlocked(ICardInstance card, Game game);

        /// <summary>
        /// Performs the actual modification on a matched card that was checked elsewhere for relevant blocked effects.
        /// </summary>
        protected abstract CardBase ApplyToCard(CardBase card, Game game);
    }
}
