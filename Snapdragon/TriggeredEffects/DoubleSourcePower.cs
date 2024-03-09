using Snapdragon.Calculations;
using Snapdragon.TargetFilters;

namespace Snapdragon.TriggeredEffects
{
    public record DoubleSourcePower<TEvent>() : ISourceTriggeredEffectBuilder<ICard, TEvent>
    {
        public IEffect Build(Game game, TEvent e, ICard source)
        {
            return new Effects.AddPowerToCards(
                new SpecificCard(source),
                new SpecificCardPower(source)
            );
        }
    }
}
