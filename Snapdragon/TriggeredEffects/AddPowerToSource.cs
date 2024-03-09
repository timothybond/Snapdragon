namespace Snapdragon.TriggeredEffects
{
    public record AddPowerToSource<TEvent>(int Amount)
        : ISourceTriggeredEffectBuilder<ICard, TEvent>
    {
        public IEffect Build(Game game, TEvent e, ICard source)
        {
            return new Effects.AddPowerToCard(source, Amount);
        }
    }
}
