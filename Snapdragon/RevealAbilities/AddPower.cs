using Snapdragon.Calculations;

namespace Snapdragon.RevealAbilities
{
    public record AddPower(ICardFilter<Card> Filter, IPowerCalculation<Card> Power)
        : IRevealAbility<Card>
    {
        public AddPower(ICardFilter<Card> Filter, int Power)
            : this(Filter, new ConstantPower(Power)) { }

        public GameState Activate(GameState game, Card source)
        {
            var cards = game
                .AllCards.Where(c => Filter.Applies(c, source, game))
                .Select(c => c with { Power = c.Power + Power.GetValue(game, source, c) });
            return game.WithCards(cards);
        }
    }
}
