using System.Collections.Immutable;

namespace Snapdragon
{
    public record Location(
        string Name,
        Column Column,
        ImmutableList<Card> TopPlayerCards,
        ImmutableList<Card> BottomPlayerCards,
        ImmutableList<Sensor<Card>> Sensors,
        bool Revealed = false
    )
    {
        public Location(string Name, Column Column)
            : this(Name, Column, [], [], []) { }

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

        public IEnumerable<Card> AllCards => this.TopPlayerCards.Concat(this.BottomPlayerCards);

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

        /// <summary>
        /// Removes a given <see cref="Card"/> from the Location.
        ///
        /// Note that this does not apply any other game logic - it should be called
        /// by something that's orchestrating whatever is supposed to happen with the <see cref="Card"/>.
        ///
        /// Note also that the <see cref="Card"/> is matched by Id to ensure nothing weird happens.
        /// </summary>
        public Location WithRemovedCard(Card card)
        {
            // TODO: Consider checking that the Card.State is correct
            switch (card.Side)
            {
                case Side.Top:
                    return this with
                    {
                        TopPlayerCards = this.TopPlayerCards.RemoveAll(c => c.Id == card.Id)
                    };
                case Side.Bottom:
                    return this with
                    {
                        BottomPlayerCards = this.BottomPlayerCards.RemoveAll(c => c.Id == card.Id)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public Location WithTemporaryCardEffect(Sensor<Card> temporaryCardEffect)
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
                TemporaryCardEffects = this.TemporaryCardEffects.RemoveAll(t =>
                    t.Id == temporaryCardEffectId
                )
            };
        }
    }
}
