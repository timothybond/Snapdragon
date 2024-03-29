namespace Snapdragon.Fluent.Calculations
{
    public record CardPower<TContext>(ISingleItemSelector<ICardInstance, TContext> Selector)
        : ICalculation<TContext>
        where TContext : class
    {
        public int GetValue(TContext context, Game game)
        {
            var cards = Selector.Get(context, game).ToList();

            if (cards.Count != 1)
            {
                throw new InvalidOperationException(
                    $"Tried to calculate the power of a card, but {cards.Count} cards were selected."
                );
            }

            return cards[0].Power;
        }
    }
}
