namespace Snapdragon.TemporaryEffects
{
    public record TemporaryEffectBuilder(TemporaryEffectTriggeredAbilityBuilder AbilityBuilder)
    {
        public TemporaryEffect<Card> Build(Game game, Card source)
        {
            var column =
                source.Column
                ?? throw new InvalidOperationException(
                    "Attempted to build a temporary effect from a card that isn't in play."
                );

            var temporaryEffect = new TemporaryEffect<Card>(
                Ids.GetNext<TemporaryEffect<Card>>(),
                column,
                source.Side,
                source,
                null
            );

            var ability = AbilityBuilder.Build(game, temporaryEffect);
            temporaryEffect = temporaryEffect with { Ability = ability };

            return temporaryEffect;
        }
    }
}
