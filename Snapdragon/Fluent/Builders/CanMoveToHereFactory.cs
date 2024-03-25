using Snapdragon.MoveAbilities;

namespace Snapdragon.Fluent.Builders
{
    public record CanMoveToHereFactory<TContext>() : IMoveAbilityFactory<TContext>
        where TContext : IObjectWithColumn
    {
        public IMoveAbility<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new CanMoveToHere<TContext>(condition);
        }
    }
}
