namespace Snapdragon
{
    public interface ISensorTriggeredAbility
    {
        Game ProcessEvent(Game game, Event e, Sensor<ICard> source);
    }

    public abstract record BaseSensorTriggeredAbility<TEvent> : ITriggeredAbility<Sensor<ICard>>
        where TEvent : Event
    {
        public Game ProcessEvent(Game game, Event e, Sensor<ICard> source)
        {
            if (e is TEvent specificEvent)
            {
                return ProcessEvent(game, specificEvent, source);
            }

            return game;
        }

        protected abstract Game ProcessEvent(Game game, TEvent e, Sensor<ICard> source);
    }
}
