namespace Snapdragon.Effects
{
    public abstract record ModifyCard(ICardInstance Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var actualCard = game.GetCardInstance(Card.Id);

            if (actualCard == null)
            {
                return game;
            }

            var updatedCard = this.ApplyToCard(actualCard, game);
            return game.WithModifiedCard(updatedCard);
        }

        /// <summary>
        /// Performs the actual modification on a matched card that was checked elsewhere for relevant blocked effects.
        /// </summary>
        protected abstract ICardInstance ApplyToCard(ICardInstance card, Game game);
    }
}
