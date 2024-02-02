using Snapdragon.Calculations;

namespace Snapdragon.RevealAbilities
{
    // TODO: Consider moving the CardState logic to the filter itself
    public record AddPower(
        ICardFilter<Card> Filter,
        IPowerCalculation<Card> Power,
        CardState State = CardState.InPlay
    ) : IRevealAbility<Card>
    {
        public AddPower(ICardFilter<Card> filter, int power)
            : this(filter, new ConstantPower<Card>(power)) { }

        public AddPower(ICardFilter<Card> filter, int power, CardState state)
            : this(filter, new ConstantPower<Card>(power), state) { }

        public Game Activate(Game game, Card source)
        {
            IEnumerable<Card> cards;

            switch (State)
            {
                case CardState.InPlay:
                    cards = game.AllCards;
                    break;
                case CardState.InLibrary:
                    cards = game[Side.Top].Library.Cards.Concat(game[Side.Bottom].Library.Cards);
                    break;
                case CardState.InHand:
                    cards = game[Side.Top].Hand.Concat(game[Side.Bottom].Hand);
                    break;
                default:
                    throw new NotImplementedException();
            }

            cards = cards.Where(c => Filter.Applies(c, source, game));

            var effects = cards.Select(c =>
            {
                var power = Power.GetValue(game, source, c);
                return new Effects.AddPowerToCard(c, power);
            });

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
