using Snapdragon.MoveAbilities;

namespace Snapdragon.Sensors
{
    public record CanMoveToHereNextTurnBuilder<T> : ISensorMoveAbilityBuilder<T>
    {
        public IMoveAbility<Sensor<T>> Build(Game game, T source)
        {
            return new CanMoveToHereOnTurn<Sensor<T>>(game.Turn + 1);
        }
    }
}
