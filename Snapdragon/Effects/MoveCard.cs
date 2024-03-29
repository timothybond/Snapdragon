namespace Snapdragon.Effects
{
    /// <summary>
    /// Moves a <see cref="Card"/> from one <see cref="Column"/> to another.
    ///
    /// Validates that the card is in the given <see cref="Column"/> initially,
    /// there are slots open in the destination <see cref="Column"/>,
    /// there are no active effects blocking the move, and that there is
    /// some move ability that applies here, although that final check
    /// can be overridden with the "Forced" argument, which is for cases
    /// where some instantly-triggered effect (usually a reveal effect)
    /// moves things rather than a <see cref="PlayerActions.MoveCardAction"/>.
    /// </summary>
    public record MoveCard(ICardInstance Card, Column From, Column To, bool Forced = false) : IEffect
    {
        public Game Apply(Game game)
        {
            var actualCard = game.Kernel[Card.Id] as ICard;

            if (actualCard == null)
            {
                return game;
            }

            // TODO: Consider whether this is an appropriate requirement
            if (actualCard.Column != From)
            {
                return game;
            }

            if (From == To)
            {
                return game;
            }

            // TODO: handle restrictions on number of cards
            if (game[To][Card.Side].Count >= Max.CardsPerLocation)
            {
                return game;
            }

            if (!game.CanMove(actualCard, To) && !Forced)
            {
                return game;
            }

            var oldLocation = game[From];
            var newLocation = game[To];

            if (
                game.GetBlockedEffects(oldLocation.Column, actualCard.Side)
                    .Contains(EffectType.MoveFromLocation)
            )
            {
                return game;
            }

            if (
                game.GetBlockedEffects(newLocation.Column, actualCard.Side)
                    .Contains(EffectType.MoveToLocation)
            )
            {
                return game;
            }

            if (game.GetBlockedEffects(actualCard).Contains(EffectType.MoveCard))
            {
                return game;
            }

            return game.MoveCard(actualCard, To);
        }
    }
}
