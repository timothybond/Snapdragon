namespace Snapdragon.RevealAbilities
{
    /// <summary>
    /// The owning player discards a random card from their hand.
    ///
    /// If specified, the cards are first filtered, and then a random one is chosen.
    ///
    /// Does not affect the opponent.
    /// </summary>
    public class DiscardCard(ICardFilter<ICard>? Filter = null) : IRevealAbility<ICard>
    {
        public Game Activate(Game game, ICard source)
        {
            List<ICardInstance> applicableCards;

            if (Filter == null)
            {
                applicableCards = game[source.Side].Hand.ToList();
            }
            else
            {
                applicableCards = game[source.Side]
                    .Hand.Where(c => Filter.Applies(c, source, game))
                    .ToList();
            }

            if (applicableCards.Count == 0)
            {
                return game;
            }

            var effect = new Effects.DiscardCard(Random.Of(applicableCards));
            return effect.Apply(game);
        }
    }
}
