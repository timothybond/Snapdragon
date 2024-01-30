using Snapdragon.Calculations;

namespace Snapdragon.RevealAbilities
{
    public record AddPowerRandomly(ICardFilter<Card> Filter, IPowerCalculation<Card> Power, int Count) : IRevealAbility<Card>
    {
        public AddPowerRandomly(ICardFilter<Card> filter, int power, int count) : this(
            filter,
            new ConstantPower(power),
            count)
        {
        }

        public Game Activate(Game game, Card source)
        {
            var cards = game
                .AllCards
                .Where(c => Filter.Applies(c, source, game))
                .OrderBy(c => Random.Next())
                .Take(Count)
                .Select(c => c with { Power = c.Power + Power.GetValue(game, source, c) });

            return game.WithCards(cards);
        }
    }
}
