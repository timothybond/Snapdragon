namespace Snapdragon.TriggeredEffects
{
    public record AddCopiesToHand<TEvent>(int Count, Func<Card, Card>? Transform = null)
        : ISourceTriggeredEffectBuilder<Card, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Card source)
        {
            return new Effects.AddCopiesToHand(source, Count, Transform);
        }
    }
}
