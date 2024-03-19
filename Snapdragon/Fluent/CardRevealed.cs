using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public record CardRevealed : Builder<OnReveal<Card>, Card, IEffectBuilder<Card>>
    {
        public CardRevealed()
            : base(new OnRevealFactory()) { }

        private class OnRevealFactory : IResultFactory<OnReveal<Card>, Card, IEffectBuilder<Card>>
        {
            public OnReveal<Card> Build(
                IEffectBuilder<Card> outcome,
                ICondition<Card>? condition = null
            )
            {
                return new OnReveal<Card>(outcome, condition);
            }
        }
    }
}
