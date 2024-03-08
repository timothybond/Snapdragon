using Snapdragon.Events;

namespace Snapdragon.RevealAbilities
{
    public class MergeWithRandomCard : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            Column column =
                source.Column
                ?? throw new InvalidOperationException(
                    "Activated MergeWithRandomCard ability on a card not in play."
                );

            source = game[column][source.Side].Single(c => c.Id == source.Id);

            var otherCardsHere = game[column][source.Side].Where(c => c.Id != source.Id).ToList();

            if (otherCardsHere.Count == 0)
            {
                // Nobody to merge with
                return game;
            }

            var mergeTarget = Random.Of(otherCardsHere);

            // TODO: Consider moving this to an IEffect, but because it's pretty complex I'm initially doing it here
            var location = game[column];

            // TODO: Make sure this triggers effects appropriately
            location = location.WithRemovedCard(source);
            return game.WithLocation(location)
                .WithModifiedCard(mergeTarget, c => c with { Power = c.Power + source.Power })
                .WithEvent(new CardMergedEvent(game.Turn, source, mergeTarget))
                .WithEvent(new CardRevealedEvent(game.Turn, mergeTarget));
        }
    }
}
