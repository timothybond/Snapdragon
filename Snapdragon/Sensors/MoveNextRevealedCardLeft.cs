using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.Sensors
{
    public class MoveNextRevealedCardLeft
        : ISourceTriggeredEffectBuilder<Sensor<ICard>, CardRevealedEvent>
    {
        public IEffect Build(Game game, CardRevealedEvent e, Sensor<ICard> source)
        {
            var nextRevealEvent = this.GetNextRevealEvent(game, source.Source);

            if (nextRevealEvent != e)
            {
                return new NullEffect();
            }

            // Can't trigger on itself (this only matters for Hulkbuster merges, I believe)
            if (nextRevealEvent.Card.Id == source.Source.Id)
            {
                return new NullEffect();
            }

            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == nextRevealEvent.Card.Id);

            if (actualCard == null)
            {
                // Card is possibly no longer in play (this can happen, e.g. Hulkbuster)
                return new NullEffect();
            }

            return new MoveCardLeft(actualCard);
        }

        /// <summary>
        /// Gets the <see cref="CardInstance"/> that was revealed next after the given one.
        /// </summary>
        /// <param name="previous">The <see cref="CardInstance"/> after which to search.</param>
        /// <returns></returns>
        private CardRevealedEvent GetNextRevealEvent(Game game, ICard previous)
        {
            CardRevealedEvent? nextRevealEvent = null;
            ICardInstance? mergedInto = null;

            // Skip all events until the source for this effect is revealed, and then that event as well
            var eventsAfterPreviousCardRevealed = game
                .PastEvents.Concat(game.NewEvents)
                .SkipWhile(e =>
                    e.Type != EventType.CardRevealed
                    || (e as CardRevealedEvent)?.Card.Id != previous.Id
                )
                .Skip(1);

            foreach (var remainingEvent in eventsAfterPreviousCardRevealed)
            {
                if (
                    remainingEvent is CardRevealedEvent cardRevealed
                    && cardRevealed.Card.Side == previous.Side
                    && nextRevealEvent == null
                )
                {
                    nextRevealEvent = cardRevealed;
                }

                if (
                    remainingEvent is CardMergedEvent cardMerged
                    && nextRevealEvent != null
                    && cardMerged.Merged.Id == nextRevealEvent.Card.Id
                )
                {
                    mergedInto = cardMerged.Target;
                }

                // This handles a very specific scenario where the next card played is Hulkbuster,
                // and the effect triggers on the card it merges onto.
                if (
                    remainingEvent is CardRevealedEvent mergedCardRevealed
                    && mergedCardRevealed.Card.Id == mergedInto?.Id
                )
                {
                    nextRevealEvent = mergedCardRevealed;
                }
            }

            if (nextRevealEvent == null)
            {
                throw new InvalidOperationException(
                    "MoveNextRevealedCardLeft was triggered, but could not find a newly-revealed card."
                );
            }

            return nextRevealEvent;
        }
    }
}
