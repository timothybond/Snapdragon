using Snapdragon.Effects;

namespace Snapdragon.Sensors
{
    public record GiveParentPowerBuilder<TEvent>(int Amount)
        : ISourceTriggeredEffectBuilder<Sensor<Card>, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Sensor<Card> source)
        {
            return new AddPowerToCard(source.Source, Amount);
        }
    }
}
