namespace Snapdragon.Fluent.Filters
{
    public record LocationsWithOpenSlots<TContext>(
        ISingleItemSelector<Player, TContext> PlayerSelector
    ) : IFilter<Location, TContext>
    {
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
