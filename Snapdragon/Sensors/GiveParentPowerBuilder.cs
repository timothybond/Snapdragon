using Snapdragon.Calculations;
using Snapdragon.Effects;
using Snapdragon.TargetFilters;

namespace Snapdragon.Sensors
{
    public record GiveParentPowerBuilder(int Amount) : ISourceTriggeredEffectBuilder<Sensor<Card>>
    {
        public IEffect Build(Game game, Event e, Sensor<Card> source)
        {
            return new AddPowerTo(new ParentCard(source), new Constant(this.Amount));
        }
    }
}
