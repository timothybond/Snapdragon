namespace Snapdragon.TriggeredEffects
{
    public record AddCopiesToHand(int Count, Func<CardInstance, CardInstance>? Transform = null)
        : ISourceTriggeredEffectBuilder<ICard, Event>
    {
        public IEffect Build(Game game, Event e, ICard source)
        {
            return new Effects.AddCopiesToHand(source, Count, Transform);
        }
    }
}
