using Snapdragon.Fluent;

namespace Snapdragon.MoveAbilities
{
    public record CanMoveToHere<T>(ICondition<T>? Condition = null) : IMoveAbility<T>
        where T : IObjectWithColumn
    {
        public bool CanMove(ICard target, T source, Column destination, Game game)
        {
            return destination == source.Column && (Condition?.IsMet(source, game) ?? true);
        }
    }
}
