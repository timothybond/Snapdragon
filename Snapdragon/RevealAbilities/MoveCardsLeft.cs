namespace Snapdragon.RevealAbilities
{
    public record MoveCardsLeft(ICardFilter<ICard> Filter) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            var cardsToMove = game
                .AllCards.Where(c => Filter.Applies(c, source, game))
                .OrderBy(c => (int)c.Column);

            // TODO: Decide if we should randomize the order these or something
            var effects = cardsToMove.Select(c => new Effects.MoveCardLeft(c));

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
