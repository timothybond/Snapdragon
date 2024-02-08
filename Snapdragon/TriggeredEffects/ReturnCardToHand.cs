namespace Snapdragon.TriggeredEffects
{
    /// <summary>
    /// Returns the card that triggered the effect to its owner's hand,
    /// optionally performing a transformation on it.
    /// </summary>
    public record ReturnCardToHand<TEvent>(Func<Card, Card>? Transform)
        : ISourceTriggeredEffectBuilder<Card, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Card source)
        {
            return new Effects.ReturnCardToHand(source, this.Transform);
        }
    }
}
