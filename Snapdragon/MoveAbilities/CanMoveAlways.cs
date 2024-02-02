namespace Snapdragon.MoveAbilities
{
    public record CanMoveAlways : IMoveAbility
    {
        public bool CanMove(Card self, Game game)
        {
            return true;
        }
    }
}
