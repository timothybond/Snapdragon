namespace Snapdragon.TriggeredEffects
{
    public record AddPowerToSource<TEvent>(int Amount)
        : ISourceTriggeredEffectBuilder<ICardInstance, TEvent>
    {
        public IEffect Build(Game game, TEvent e, ICardInstance source)
        {
            return new Effects.AddPowerToCard(source, Amount);
        }
    }
}
