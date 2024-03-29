using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public record CardRevealed
        : Builder<OnReveal<ICard>, ICard, IEffectBuilder<ICard>>
    {
        public CardRevealed()
            : base(new OnRevealFactory()) { }

        private class OnRevealFactory
            : IResultFactory<
                OnReveal<ICard>,
                ICard,
                IEffectBuilder<ICard>
            >
        {
            public OnReveal<ICard> Build(
                IEffectBuilder<ICard> outcome,
                ICondition<ICard>? condition = null
            )
            {
                return new OnReveal<ICard>(outcome, condition);
            }
        }
    }
}
