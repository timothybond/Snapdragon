using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.TriggeredEffects
{
    public record AddCopyToOldLocation : ISourceTriggeredEffectBuilder<ICard, CardMovedEvent>
    {
        public IEffect Build(Game game, CardMovedEvent e, ICard source)
        {
            return new AddCopyToLocation(source, e.From);
        }
    }
}
