using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.TriggeredEffects
{
    public record AddCopyToOldLocation : ISourceTriggeredEffectBuilder<ICardInstance, CardMovedEvent>
    {
        public IEffect Build(Game game, CardMovedEvent e, ICardInstance source)
        {
            return new AddCopyToLocation(source, e.From);
        }
    }
}
