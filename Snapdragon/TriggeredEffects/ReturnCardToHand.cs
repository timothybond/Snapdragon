namespace Snapdragon.TriggeredEffects
{
    /// <summary>
    /// Returns the card that owns the effect to its owner's hand,
    /// optionally performing a transformation on it.
    /// </summary>
    public record ReturnCardToHand<TEvent>(Func<CardInstance, CardInstance>? Transform)
        : ISourceTriggeredEffectBuilder<ICard, TEvent>
    {
        public IEffect Build(Game game, TEvent e, ICard source)
        {
            return new Effects.ReturnCardToHand(source, this.Transform);
        }
    }
}
