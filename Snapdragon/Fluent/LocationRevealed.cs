using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public record LocationRevealed : Builder<OnReveal<Location>, Location, IEffectBuilder<Location>>
    {
        public LocationRevealed()
            : base(new OnRevealFactory()) { }

        private class OnRevealFactory
            : IResultFactory<OnReveal<Location>, Location, IEffectBuilder<Location>>
        {
            public OnReveal<Location> Build(
                IEffectBuilder<Location> outcome,
                ICondition<Location>? condition = null
            )
            {
                return new OnReveal<Location>(outcome, condition);
            }
        }
    }
}
