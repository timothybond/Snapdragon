namespace Snapdragon.RevealAbilities
{
    public record MoveCardsToSelf(ICardFilter<Card> Filter) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var cardsToMove = game.AllCards.Where(c =>
                Filter.Applies(c, source, game) && c.Column != source.Column
            );
            var effects = new List<IEffect>();

            foreach (var card in cardsToMove)
            {
                effects.Add(new Effects.MoveCard(card, card.Column, source.Column, true));
            }

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
