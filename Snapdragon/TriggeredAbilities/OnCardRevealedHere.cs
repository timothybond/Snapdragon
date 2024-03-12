using Snapdragon.Events;

namespace Snapdragon.TriggeredAbilities
{
    public record OnCardRevealedHere(ICardEventEffectBuilder EffectBuilder)
        : ITriggeredAbility<Location>
    {
        public Game ProcessEvent(Game game, Event e, Location source)
        {
            if (e is CardRevealedEvent cardRevealed && cardRevealed.Card.Column == source.Column)
            {
                var effect = EffectBuilder.Build(cardRevealed);
                return effect.Apply(game);
            }

            return game;
        }
    }
}
