namespace Snapdragon.TriggeredEffects
{
    public record AddCopiesToHand<TEvent>(int Count, Func<CardInstance, CardInstance>? Transform = null)
        : ISourceTriggeredEffectBuilder<ICard, TEvent>
    {
        public IEffect Build(Game game, TEvent e, ICard source)
        {
            return new Effects.AddCopiesToHand(source, Count, Transform);
        }
    }
}
