using Snapdragon.Calculations;

namespace Snapdragon.RevealAbilities
{
    public record AddPower(ICardFilter<Card> Filter, IPowerCalculation<Card> Power)
        : IRevealAbility<Card>
    {
        public AddPower(ICardFilter<Card> Filter, int Power)
            : this(Filter, new ConstantPower<Card>(Power)) { }

        public Game Activate(Game game, Card source)
        {
            var cards = game.AllCards.Where(c => Filter.Applies(c, source, game));

            var effects = cards.Select(c =>
            {
                var power = Power.GetValue(game, source, c);
                return new Effects.AddPowerToCard(c, power);
            });

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
