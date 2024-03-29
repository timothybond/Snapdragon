namespace Snapdragon.Effects
{
    /// <summary>
    /// Effect where a given player adds a random <see cref="CardInstance"/> to their hand.
    /// </summary>
    public record AddRandomCardToHand(Side Side, ICardDefinitionFilter Filter) : IEffect
    {
        public Game Apply(Game game)
        {
            // Note we normally enforce hand-size limit elsewhere.
            // TODO: find a way to centralize this
            if (game[Side].Hand.Count >= Max.HandSize)
            {
                return game;
            }

            var randomCardDefinition = Random.Of(SnapCards.All.Where(Filter.Applies).ToList());

            return game.WithNewCardInHand(randomCardDefinition, Side);

        }
    }
}
