namespace Snapdragon.MoveAbilities
{
    public record CanMoveAlways : IMoveAbility<ICardInstance>
    {
        public bool CanMove(ICard target, ICardInstance source, Column destination, Game game)
        {
            return target.Id == source.Id;
        }
    }
}
