namespace Snapdragon.RevealAbilities
{
    public record DestroyRandomCardsInPlay<T>(ICardFilter<T> Filter, int Count) : IRevealAbility<T>
    {
        public Game Activate(Game game, T source)
        {
            var possibleCards = game.AllCards.Where(c =>
                Filter.Applies(c, source, game)
                && !game.GetBlockedEffects(c).Contains(EffectType.DestroyCard)
            );

            var selectedCards = possibleCards.OrderBy(c => Random.Next()).Take(Count);

            var effects = selectedCards.Select(c => new Effects.DestroyCardInPlay(c));

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
