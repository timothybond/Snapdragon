using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record AddCardToLocationBuilder<TContext>(
        CardDefinition CardDefinition,
        ISelector<Location, TContext> LocationSelector,
        ISelector<Player, TContext> PlayerSelector
    ) : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var locations = LocationSelector.Get(context, game);
            List<Player> players;

            if (PlayerSelector == null)
            {
                if (context is IObjectWithSide objectWithSide)
                {
                    players = new List<Player>
                    {
                        game[objectWithSide.Side].Player
                    };
                }
                else
                {
                    throw new InvalidOperationException(
                        $"{nameof(PlayerSelector)} must be specified if "
                            + $"the context does not implement {nameof(IObjectWithSide)}."
                    );
                }
            }
            else
            {
                players = PlayerSelector.Get(context, game).ToList();
            }

            return new AndEffect(
                players.SelectMany(player =>
                    locations.Select(loc => new AddCardToLocation(
                        CardDefinition,
                        player.Side,
                        loc.Column
                    ))
                )
            );
        }
    }

    public static class AddCardToLocationExtensions
    {
        public static AddCardToLocationBuilder<TContext> AddCard<TContext>(
            this ISelector<Location, TContext> locationSelector,
            CardDefinition cardDefinition,
            ISelector<Player, TContext>? playerSelector = null
        )
            where TContext : class
        {
            return new AddCardToLocationBuilder<TContext>(
                cardDefinition,
                locationSelector,
                playerSelector
            );
        }
    }
}
