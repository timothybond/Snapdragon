using Snapdragon.TargetFilters;

namespace Snapdragon.TriggeredEffects
{
    public record AddPowerToSource(int Amount) : ISourceTriggeredEffectBuilder<Card>
    {
        public IEffect Build(Game game, Event e, Card source)
        {
            return new Effects.AddPowerTo(
                new SpecificCard(source),
                new ConstantCalculation(Amount)
            );
        }
    }
}
