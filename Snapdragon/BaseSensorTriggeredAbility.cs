namespace Snapdragon
{
    public interface ISensorTriggeredAbility
    {
        Game ProcessEvent(Game game, Event e, Sensor<Card> source);
    }

    public abstract record BaseSensorTriggeredAbility<TEvent> : ISensorTriggeredAbility
        where TEvent : Event
    {
        public Game ProcessEvent(Game game, Event e, Sensor<Card> source)
        {
            if (e is TEvent specificEvent)
            {
                return ProcessEvent(game, specificEvent, source);
            }

            return game;
        }

        protected abstract Game ProcessEvent(Game game, TEvent e, Sensor<Card> source);
    }
}
