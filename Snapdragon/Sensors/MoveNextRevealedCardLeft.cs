using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.Sensors
{
    public class MoveNextRevealedCardLeft
        : ISourceTriggeredEffectBuilder<Sensor<Card>, CardRevealedEvent>
    {
        public IEffect Build(Game game, CardRevealedEvent e, Sensor<Card> source)
        {
            var nextRevealedCard = this.GetNextRevealedCard(game, source.Source);

            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == nextRevealedCard.Id);

            if (actualCard == null)
            {
                // Card is possibly no longer in play (this can happen, e.g. Hulkbuster)
                return new NullEffect();
            }

            return new MoveCardLeft(actualCard);
        }

        /// <summary>
        /// Gets the <see cref="Card"/> that was revealed next after the given one.
        /// </summary>
        /// <param name="previous">The <see cref="Card"/> after which to search.</param>
        /// <returns></returns>
        private Card GetNextRevealedCard(Game game, Card previous)
        {
            var cardRevealedEvents = game
                .PastEvents.Concat(game.NewEvents)
                .OfType<CardRevealedEvent>()
                .Where(e => e.Card.Side == previous.Side)
                .ToList();

            var nextRevealEvent = cardRevealedEvents
                .SkipWhile(c => c.Card.Id != previous.Id)
                .Skip(1)
                .FirstOrDefault();

            if (nextRevealEvent == null)
            {
                throw new InvalidOperationException(
                    "MoveNextRevealedCardLeft was triggered, but could not find a newly-revealed card."
                );
            }

            return nextRevealEvent.Card;
        }
    }
}
