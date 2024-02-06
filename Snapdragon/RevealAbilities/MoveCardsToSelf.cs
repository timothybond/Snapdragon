namespace Snapdragon.RevealAbilities
{
    public record MoveCardsToSelf(ICardFilter<Card> Filter) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            if (source.Column == null)
            {
                throw new InvalidOperationException(
                    "MoveCardsToSelf triggered on a card with no Column value."
                );
            }

            var cardsToMove = game.AllCards.Where(c =>
                Filter.Applies(c, source, game) && c.Column != source.Column
            );
            var effects = new List<IEffect>();

            foreach (var card in cardsToMove)
            {
                if (card.Column == null)
                {
                    throw new InvalidOperationException(
                        "Found a card in play with no Column value."
                    );
                }

                effects.Add(
                    new Effects.MoveCard(card, card.Column.Value, source.Column.Value, true)
                );
            }

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
