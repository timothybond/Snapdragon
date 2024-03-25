namespace Snapdragon.Effects
{
    public record CreateMoveSensor(
        Card Source,
        IMoveAbility<Sensor<Card>> MoveAbility,
        ITriggeredAbility<Sensor<Card>>? TriggeredAbility = null
    ) : IEffect
    {
        public Game Apply(Game game)
        {
            var sensor = new Sensor<Card>(
                Ids.GetNext<Sensor<Card>>(),
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
