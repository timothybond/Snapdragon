using Snapdragon.Calculations;

namespace Snapdragon.RevealAbilities
{
    // TODO: Consider moving the CardState logic to the filter itself
    public record AddPower(
        ICardFilter<ICard> Filter,
        IPowerCalculation<ICard> Power,
        CardState State = CardState.InPlay
    ) : IRevealAbility<Card>
    {
        public AddPower(ICardFilter<ICard> filter, int power)
            : this(filter, new Constant<ICard>(power)) { }

        public AddPower(ICardFilter<ICard> filter, int power, CardState state)
            : this(filter, new Constant<ICard>(power), state) { }

        public Game Activate(Game game, Card source)
        {
            IEnumerable<ICard> cards;

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
