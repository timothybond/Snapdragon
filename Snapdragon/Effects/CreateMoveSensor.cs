namespace Snapdragon.Effects
{
    public record CreateMoveSensor(
        ICard Source,
        IMoveAbility<Sensor<ICard>> MoveAbility,
        ITriggeredAbility<Sensor<ICard>>? TriggeredAbility = null
    ) : IEffect
    {
        public Game Apply(Game game)
        {
            var sensor = new Sensor<ICard>(
                Ids.GetNextSensor(),
                Source.Column,
                Source.Side,
                Source,
                TriggeredAbility,
                MoveAbility,
                game.Turn
            );

            return game.WithSensor(sensor);
        }
    }
}
