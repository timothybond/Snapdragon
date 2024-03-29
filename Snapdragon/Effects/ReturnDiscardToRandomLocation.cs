namespace Snapdragon.Effects
{
    /// <summary>
    /// Returns a discarded <see cref="ICardInstance"/> to play at a random <see cref="Column"/>.
    /// </summary>
    public record ReturnDiscardToRandomLocation(ICardInstance Card) : IEffect
    {
        public Game Apply(Game game)
        {
            // Determine if there is an open slot
            // TODO: Handle any restrictions on slots available
            var columnsWithSpace = All
                .Columns.Where(col =>
                    game[col][Card.Side].Count < Max.CardsPerLocation
                    && !game.GetBlockedEffects(col, Card.Side).Contains(EffectType.AddCard)
                )
                .ToList();

            if (columnsWithSpace.Count == 0)
            {
                return game;
            }

            var targetColumn = Random.Of(columnsWithSpace);

            var returnEffect = new ReturnDiscardToLocation(Card, targetColumn);
            return returnEffect.Apply(game);
        }
    }
}
