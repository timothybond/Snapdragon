using Snapdragon.Events;

namespace Snapdragon.Fluent.Filters
{
    public record NextRevealedCard<TContext>(
        ISingleItemSelector<ICardInstance, TContext> PriorCard,
        ISingleItemSelector<ICardInstance, TContext>? Ignored = null
    ) : IEventFilter<CardRevealedEvent, TContext>
    {
        public bool Includes(CardRevealedEvent e, TContext context, Game game)
        {
            // Note: this logic is so complex courtesy of Hulkbuster.
            // If the next played card after Iron Fist (or anything else
            // that triggers on next reveal, presumably) is Hulkbuster,
            // then Hulkbuster DOES get revealed, but Iron Fist subsequently
            // triggers on the merged card. Unless the merged card is Iron Fist.
            var priorCard = PriorCard.GetOrDefault(context, game);

            if (priorCard == null)
            {
                return false;
            }

            var eventsAfterPreviousCardRevealed = game
                .PastEvents.Concat(game.NewEvents)
                .SkipWhile(e =>
                    e.Type != EventType.CardRevealed
                    || (e as CardRevealedEvent)?.Card.Id != priorCard.Id
                )
                .Skip(1);

            CardRevealedEvent? nextRevealEvent = null;
            ICardInstance? mergedInto = null;

            foreach (var remainingEvent in eventsAfterPreviousCardRevealed)
            {
                if (
                    remainingEvent is CardRevealedEvent cardRevealed
                    && cardRevealed.Card.Side == priorCard.Side
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
                return false;
            }

            if (nextRevealEvent.Card.Id == Ignored?.GetOrDefault(context, game)?.Id)
            {
                return false;
            }

            return nextRevealEvent == e;
        }
    }
}
