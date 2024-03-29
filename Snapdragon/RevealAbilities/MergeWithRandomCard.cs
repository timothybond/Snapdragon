using Snapdragon.Events;

namespace Snapdragon.RevealAbilities
{
    public class MergeWithRandomCard : IRevealAbility<ICard>
    {
        public Game Activate(Game game, ICard source)
        {
            source = game[source.Column][source.Side].Single(c => c.Id == source.Id);

            var otherCardsHere = game[source.Column]
                [source.Side]
                .Where(c => c.Id != source.Id)
                .ToList();

            if (otherCardsHere.Count == 0)
            {
                // Nobody to merge with
                return game;
            }

            var mergeTarget = Random.Of(otherCardsHere);

            // TODO: Consider moving this to an IEffect, but because it's pretty complex I'm initially doing it here
            var location = game[source.Column];

            game = game.RemoveCard(source)
                .WithUpdatedCard(
                    mergeTarget.Base with
                    {
                        Power = mergeTarget.Base.Power + source.Power
                    }
                )
                .WithEvent(new CardMergedEvent(game.Turn, source, mergeTarget))
                .WithEvent(new CardRevealedEvent(game.Turn, mergeTarget));

            return game;
        }
    }
}
