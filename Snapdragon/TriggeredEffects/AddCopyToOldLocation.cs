using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.TriggeredEffects
{
    public record AddCopyToOldLocation : ISourceTriggeredEffectBuilder<Card>
    {
        public IEffect Build(Game game, Event e, Card source)
        {
            // Technically this is the trigger's responsibility, but to be safe
            if (e is CardMovedEvent cardMoved && cardMoved.Card.Id == source.Id)
            {
                return new AddCopyToLocation(source, cardMoved.From);
            }
            else
            {
                throw new InvalidOperationException(
                    "AddCopyToOldLocation was triggered on an event that wasn't a CardMovedEvent."
                );
            }
        }
    }
}
