namespace Snapdragon.PlayerActions
{
    public record MoveCardAction(Side Side, ICard Card, Column From, Column To)
        : IPlayerAction
    {
        public Game Apply(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            // Sanity checks to ensure we should move the card
            if (actualCard == null)
            {
                throw new InvalidOperationException("Card was not found in play.");
            }

            if (!game.CanMove(actualCard, To))
            {
                throw new InvalidOperationException(
                    "Card does not have a currently-usable move ability."
                );
            }

            // TODO: Handle effects that limit card play
            if (game[To][Side].Count >= Max.CardsPerLocation)
            {
                throw new InvalidOperationException("Cannot move to full location.");
            }

            if (game.GetBlockedEffects(actualCard).Contains(EffectType.MoveCard))
            {
                throw new InvalidOperationException("Card is blocked from moving.");
            }

            if (game.GetBlockedEffects(From).Contains(EffectType.MoveFromLocation))
            {
                throw new InvalidOperationException("Card cannot move away from given location.");
            }

            if (game.GetBlockedEffects(To).Contains(EffectType.MoveToLocation))
            {
                throw new InvalidOperationException("Card cannot move to from given location.");
            }

            var effect = new Effects.MoveCard(actualCard, From, To);

            return effect.Apply(game);
        }

        public override string ToString()
        {
            return $"[{Side}]: Move {Card.Name} ({Card.Id}) from {From} to {To}.";
        }
    }
}
