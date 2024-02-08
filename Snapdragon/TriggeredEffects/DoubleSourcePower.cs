using Snapdragon.Calculations;
using Snapdragon.TargetFilters;

namespace Snapdragon.TriggeredEffects
{
    public record DoubleSourcePower<TEvent>() : ISourceTriggeredEffectBuilder<Card, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Card source)
        {
            return new Effects.AddPowerToCards(
                new SpecificCard(source),
                new SpecificCardPower(source)
            );
        }
    }
}
