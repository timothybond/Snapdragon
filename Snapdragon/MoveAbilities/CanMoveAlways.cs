namespace Snapdragon.MoveAbilities
{
    public record CanMoveAlways : IMoveAbility<Card>
    {
        public bool CanMove(Card target, Card source, Column destination, Game game)
        {
            return target.Id == source.Id;
        }
    }
}
