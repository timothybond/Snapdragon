namespace Snapdragon.Sensors
{
    /// <summary>
    /// A triggered ability for a sensor that deletes itself up
    /// after the specified turn.
    ///
    /// Can optionally have another triggered ability nested inside of it,
    /// if it's a sensor that has a triggered ability AND an expiration,
    /// but it's also valid to omit that if the sensor has some non-triggered
    /// ability but also goes away.
    /// </summary>
    /// <param name="Turn"></param>
    /// <param name="Source"></param>
    /// <param name="Inner"></param>
    public record ExpiringSensorTriggeredAbility(
        int Turn,
        Sensor<Card> Source,
        TriggeredSensorAbility? Inner
    ) : ISensorTriggeredAbility
    {
        // TODO: See if we can remove the need for these
        public bool InHand => false;
        public bool InDeck => false;
        public bool DiscardedOrDestroyed => false;

        public Game ProcessEvent(Game game, Event e, Sensor<Card> source)
        {
            // Note: We trigger the inner effect first because Jessica Jones triggers on "nothing played"
            game = this.Inner?.ProcessEvent(game, e, source) ?? game;

            if (e.Type == EventType.TurnEnded && e.Turn == this.Turn)
            {
                game = game.WithTemporaryCardEffectDeleted(Source.Id);
            }

            return game;
        }
    }
}
