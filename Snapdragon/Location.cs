using System.Collections.Immutable;

namespace Snapdragon
{
    public record Location(
        Column Column,
        LocationDefinition Definition,
        ImmutableList<Card> TopPlayerCards,
        ImmutableList<Card> BottomPlayerCards,
        ImmutableList<Sensor<Card>> Sensors,
        bool Revealed = false
    ) : IObjectWithColumn
    {
        public Location(Column Column, LocationDefinition Definition)
            : this(Column, Definition, [], [], []) { }

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

        Column? IObjectWithPossibleColumn.Column => Column;

        /// <summary>
        /// Adds a <see cref="CardInstance"/> to the given location.
        ///
        /// Note that this does not apply any other game logic - it should be called
        /// by something that's orchestrating whatever is supposed to happen with the <see cref="CardInstance"/>.
        /// </summary>
        public Location WithCard(ICard card)
        {
            var cardInPlay = card.InPlayAt(this.Column);

            // TODO: Consider checking that the Card.State is correct
            switch (cardInPlay.Side)
            {
                case Side.Top:
                    return this with { TopPlayerCards = this.TopPlayerCards.Add(cardInPlay) };
                case Side.Bottom:
                    return this with { BottomPlayerCards = this.BottomPlayerCards.Add(cardInPlay) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Removes a <see cref="Card"/> from the given location.
        ///
        /// Note that this does not apply any other game logic - it should be called
        /// by something that's orchestrating whatever is supposed to happen with the <see cref="Card"/>.
        /// </summary>
        public Location WithoutCard(Card card)
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

        public Location WithSensor(Sensor<Card> sensor)
        {
            return this with { Sensors = this.Sensors.Add(sensor) };
        }

        public Location WithSensorDeleted(long sensorId)
        {
            return this with { Sensors = this.Sensors.RemoveAll(t => t.Id == sensorId) };
        }
    }
}
