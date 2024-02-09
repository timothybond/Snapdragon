namespace Snapdragon.RevealAbilities
{
    public record AddCardsToLocations<T>(
        CardDefinition Definition,
        ILocationFilter<T> LocationFilter,
        ISideFilter<T> SideFilter,
        int CardsPerLocation = 1
    ) : IRevealAbility<T>
    {
        public Game Activate(Game game, T source)
        {
            var locations = new[] { game.Left, game.Middle, game.Right }.Where(l =>
                LocationFilter.Applies(l, source, game)
            );

            var sides = All.Sides.Where(s => SideFilter.Applies(s, source, game));

            var effects = locations.SelectMany(l =>
            {
                var results = new List<IEffect>();
                foreach (var side in sides)
                {
                    for (var i = 0; i < CardsPerLocation; i++)
                    {
                        results.Add(new Effects.AddCardToLocation(Definition, side, l.Column));
                    }
                }

                return results;
            });

            foreach (var effect in effects)
            {
                game = effect.Apply(game);
            }

            return game;
        }
    }
}
