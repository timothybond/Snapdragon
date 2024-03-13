using Snapdragon.Calculations;
using Snapdragon.TargetFilters;

namespace Snapdragon.TriggeredEffects
{
    public record DoubleSourcePower() : ISourceTriggeredEffectBuilder<ICard, Event>
    {
        public IEffect Build(Game game, Event e, ICard source)
        {
            return new Effects.AddPowerToCards(
                new SpecificCard(source),
                new SpecificCardPower(source)
            );
        }
    }
}
