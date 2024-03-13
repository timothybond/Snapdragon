using Snapdragon.Calculations;

namespace Snapdragon.RevealAbilities
{
    public record AddPowerRandomly<T>(ICardFilter<T> Filter, IPowerCalculation<T> Power, int Count)
        : IRevealAbility<T>
    {
        public AddPowerRandomly(ICardFilter<T> filter, int power, int count)
            : this(filter, (IPowerCalculation<T>)new Constant(power), count) { }

        public Game Activate(Game game, T source)
        {
            var cards = game
                .AllCards.Where(c =>
                {
                    if (!Filter.Applies(c, source, game))
                    {
                        return false;
                    }

                    // This looks redundant, because the AddPower effect also does this check,
                    // but I believe random power additions (e.g. Ironheart) should get three
                    // cards that CAN have their power adjusted, rather than potentially
                    // selecting a card that doesn't and "using up" one of the adjustments.
                    //
                    // However, I don't think I've actually seen this case in play, so I could be wrong.
                    var blockedEffects = game.GetBlockedEffects(c);

                    if (blockedEffects.Contains(EffectType.AdjustPower))
                    {
                        return false;
                    }

                    var power = Power.GetValue(game, source, c);
                    if (blockedEffects.Contains(EffectType.ReducePower) && power < 0)
                    {
                        return false;
                    }

                    return true;
                })
                .OrderBy(c => Random.Next())
                .Take(Count);

            var effects = cards.Select(c => new Effects.AddPowerToCard(
                c,
                Power.GetValue(game, source, c)
            ));

            return effects.Aggregate(game, (g, e) => e.Apply(g));
        }
    }
}
