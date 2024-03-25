using Snapdragon.Effects;
using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record AddCardToHandBuilder<TContext>(
        ISingleItemSelector<CardDefinition, TContext> CardDefinitionSelector,
        ISelector<Player, TContext> PlayerSelector
    ) : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cardDefinition = CardDefinitionSelector.GetOrDefault(context, game);

            if (cardDefinition == null)
            {
                return new NullEffect();
            }

            var players = PlayerSelector.Get(context, game).ToList();

            if (players.Count == 0)
            {
                return new NullEffect();
            }

            // Technically redundant
            if (players.Count == 1)
            {
                return new AddCardToHand(cardDefinition, players[0].Side);
            }

            return new AndEffect(players.Select(p => new AddCardToHand(cardDefinition, p.Side)));
        }
    }

    public static class AddToHandExtensions
    {
        public static TAbility AddToHand<TAbility, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TContext>> builder,
            ISingleItemFilter<CardDefinition, TContext> cardDefinitionFilter,
            ISelector<Player, TContext> playerSelector
        )
        {
            return builder.Build(
                new AddCardToHandBuilder<TContext>(
                    new FilteredSingleItemSelector<CardDefinition, TContext>(
                        new SnapCardsSelector<TContext>(),
                        cardDefinitionFilter
                    ),
                    playerSelector
                )
            );
        }

        public static AddCardToHandBuilder<TContext> AddToHand<TContext>(
            this ISelector<Player, TContext> playerSelector,
            ISingleItemSelector<CardDefinition, TContext> cardDefinitionSelector
        )
        {
            return new AddCardToHandBuilder<TContext>(cardDefinitionSelector, playerSelector);
        }

        public static AddCardToHandBuilder<TContext> AddToHand<TContext>(
            this ISelector<Player, TContext> playerSelector,
            ISingleItemFilter<CardDefinition, TContext> cardDefinitionFilter
        )
        {
            return new AddCardToHandBuilder<TContext>(
                new FilteredSingleItemSelector<CardDefinition, TContext>(
                    new SnapCardsSelector<TContext>(),
                    cardDefinitionFilter
                ),
                playerSelector
            );
        }
    }
}
