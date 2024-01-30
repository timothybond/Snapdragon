namespace Snapdragon.TemporaryEffects
{
    public record ExpiringTemporaryEffectTriggeredAbility(
        int Turn,
        TemporaryEffect<Card> Source,
        TriggeredAbility<TemporaryEffect<Card>> Inner) : ITriggeredAbility<TemporaryEffect<Card>>
    {
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
    }
}
