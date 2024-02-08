using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.TriggeredEffects
{
    public record AddCopyToOldLocation : ISourceTriggeredEffectBuilder<Card, CardMovedEvent>
    {
        public IEffect Build(Game game, CardMovedEvent e, Card source)
        {
            return new AddCopyToLocation(source, e.From);
        }
    }
}
