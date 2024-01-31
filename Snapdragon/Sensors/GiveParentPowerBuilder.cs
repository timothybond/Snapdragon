using Snapdragon.Effects;
using Snapdragon.TargetFilters;

namespace Snapdragon.Sensors
{
    public record GiveParentPowerBuilder(int Amount) : IEffectBuilder<Sensor<Card>>
    {
        public IEffect Build(Game game, Sensor<Card> source)
        {
            return new AddPowerTo(new ParentCard(source), new ConstantCalculation(this.Amount));
        }
    }
}
