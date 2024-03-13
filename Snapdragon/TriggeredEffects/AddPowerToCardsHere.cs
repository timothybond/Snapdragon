using Snapdragon.Effects;

namespace Snapdragon.TriggeredEffects
{
    public record AddPowerToCardsHere(int Amount)
        : ISourceTriggeredEffectBuilder<IObjectWithColumn, Event>
    {
        public IEffect Build(Game game, Event e, IObjectWithColumn source)
        {
            var cardsHere = game[source.Column].AllCards;

            IEnumerable<IEffect> addPowerEffects = cardsHere.Select(c => new AddPowerToCard(
                c,
                Amount
            ));

            return addPowerEffects.Aggregate(
                (IEffect)new NullEffect(),
                (acc, e) => new AndEffect(acc, e)
            );
        }
    }
}
