using Snapdragon.Calculations;
using Snapdragon.TargetFilters;

namespace Snapdragon.TriggeredEffects
{
    public record DoubleSourcePower() : ISourceTriggeredEffectBuilder<Card>
    {
        public IEffect Build(Game game, Event e, Card source)
        {
            return new Effects.AddPowerTo(new SpecificCard(source), new SpecificCardPower(source));
        }
    }
}
