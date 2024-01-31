namespace Snapdragon.Sensors
{
    public record ExpiringTemporaryEffectTriggeredAbility(
        int Turn,
        Sensor<Card> Source,
        TriggeredSensorAbility<Sensor<Card>> Inner
    ) : ITriggeredAbility<Sensor<Card>>
    {
        // TODO: See if we can remove the need for these
        public bool InHand => false;
        public bool InDeck => false;
        public bool DiscardedOrDestroyed => false;

        public Game ProcessEvent(Game game, Event e)
        {
            // Note: We trigger the inner effect first because Jessica Jones triggers on "nothing played"
            game = this.Inner.ProcessEvent(game, e);

            if (e.Type == EventType.TurnEnded && e.Turn == this.Turn)
            {
                game = game.WithTemporaryCardEffectDeleted(Source.Id);
            }

            return game;
        }

        public Game ProcessEvent(Game game, Event e, Sensor<Card> source)
        {
            throw new NotImplementedException();
        }
    }
}
