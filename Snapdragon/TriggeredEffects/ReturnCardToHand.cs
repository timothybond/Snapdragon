namespace Snapdragon.TriggeredEffects
{
    /// <summary>
    /// Returns the card that owns the effect to its owner's hand,
    /// optionally performing a transformation on it.
    /// </summary>
    public record ReturnCardToHand(Func<CardInstance, CardInstance>? Transform)
        : ISourceTriggeredEffectBuilder<ICard, Event>
    {
        public IEffect Build(Game game, Event e, ICard source)
        {
            return new Effects.ReturnCardToHand(source, this.Transform);
        }
    }
}
