using Snapdragon.Effects;
using Snapdragon.TargetFilters;

namespace Snapdragon.TemporaryEffects
{
    public record GiveParentPowerBuilder(int Amount) : IEffectBuilder<TemporaryEffect<Card>>
    {
        public IEffect Build(Game game, TemporaryEffect<Card> source)
        {
            return new AddPowerTo(new ParentCard(source), new ConstantCalculation(this.Amount));
        }
    }
}
