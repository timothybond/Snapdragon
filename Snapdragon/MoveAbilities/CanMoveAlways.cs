namespace Snapdragon.MoveAbilities
{
    public record CanMoveAlways : IMoveAbility<ICard>
    {
        public bool CanMove(
            ICard target,
            ICard source,
            Column destination,
            Game game
        )
        {
            return target.Id == source.Id;
        }
    }
}
