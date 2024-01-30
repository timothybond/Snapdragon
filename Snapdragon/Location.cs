using System.Collections.Immutable;

namespace Snapdragon
{
    public record Location(
        string Name,
        Column Column,
        ImmutableList<Card> TopPlayerCards,
        ImmutableList<Card> BottomPlayerCards,
        ImmutableList<TemporaryEffect<Card>> TemporaryCardEffects,
        bool Revealed = false)
    {
        public Location(string Name, Column Column) : this(Name, Column, [], [], [])
        {
        }

        public ImmutableList<Card> this[Side side]
        {
            get
            {
                switch (side)
                {
                    case Side.Top:
                        return TopPlayerCards;
                    case Side.Bottom:
                        return BottomPlayerCards;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IReadOnlyList<Card> AllCards => this.TopPlayerCards.Concat(this.BottomPlayerCards).ToList();

        public Location WithPlayedCard(Card card, Side side)
        {
            // TODO: Consider checking that the Card.State is correct
            switch (side)
            {
                case Side.Top:
                    return this with { TopPlayerCards = this.TopPlayerCards.Add(card) };
                case Side.Bottom:
                    return this with { BottomPlayerCards = this.BottomPlayerCards.Add(card) };
                default:
                    throw new NotImplementedException();
            }
        }

        public Location WithTemporaryCardEffect(TemporaryEffect<Card> temporaryCardEffect)
        {
            return this with
            {
                TemporaryCardEffects = this.TemporaryCardEffects.Add(temporaryCardEffect)
            };
        }

        public Location WithTemporaryCardEffectDeleted(int temporaryCardEffectId)
        {
            return this with
            {
                TemporaryCardEffects = this.TemporaryCardEffects.RemoveAll(t => t.Id == temporaryCardEffectId)
            };
        }
    }
}
