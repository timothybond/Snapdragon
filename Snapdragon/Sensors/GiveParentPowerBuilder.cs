using Snapdragon.Effects;

namespace Snapdragon.Sensors
{
    public record GiveParentPowerBuilder<TEvent>(int Amount)
        : ISourceTriggeredEffectBuilder<Sensor<ICard>, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Sensor<ICard> source)
        {
            return new AddPowerToCard(source.Source, Amount);
        }
    }
}
