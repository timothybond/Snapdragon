namespace Snapdragon.Fluent.Filters
{
    public record LocationsWithOpenSlots<TContext>(
        ISingleItemSelector<Player, TContext> PlayerSelector
    ) : IFilter<Location, TContext>
    {
        public bool Applies(Location item, TContext context, Game game)
        {
            var player = PlayerSelector.GetOrDefault(context, game);

            if (player == null)
            {
                throw new InvalidOperationException(
                    $"Tried to check if a Location is selected by {nameof(LocationsWithOpenSlots<TContext>)}"
                        + $" but could not determine the Player."
                );
            }

            return item[player.Side].Count < Max.CardsPerLocation;
        }

        public IEnumerable<Location> GetFrom(
            IEnumerable<Location> initial,
            TContext context,
            Game game
        )
        {
            var player = PlayerSelector.GetOrDefault(context, game);

            if (player == null)
            {
                throw new InvalidCastException(
                    $"Could not get player when invoking"
                        + $" {nameof(LocationsWithOpenSlots<TContext>)}."
                );
            }

            // TODO: Handle other slot restrictions
            return initial.Where(l => l[player.Side].Count < Max.CardsPerLocation);
        }
    }
}
