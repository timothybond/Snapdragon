using Snapdragon.Fluent.Conditions;

namespace Snapdragon.Fluent.Builders
{
    public record PastEventOfTypeConditionBuilder<TEvent, TAbility, TContext, TOutcome>(
        IResultFactory<TAbility, TContext, TOutcome> Factory
    )
    {
        public IBuilderWithCondition<TAbility, TContext, TOutcome> Where(
            IEventFilter<TEvent, TContext> filter
        )
        {
            return new BuilderWithCondition<TAbility, TContext, TOutcome>(
                new PastEventCondition<TEvent, TContext>(filter),
                Factory
            );
        }
    }
}
