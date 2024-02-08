namespace Snapdragon.TriggeredEffects
{
    public record AddPowerToSource<TEvent>(int Amount) : ISourceTriggeredEffectBuilder<Card, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Card source)
        {
            return new Effects.AddPowerToCard(source, Amount);
        }
    }
}
