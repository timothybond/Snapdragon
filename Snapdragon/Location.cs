using System.Collections.Immutable;
using Snapdragon.GameAccessors;

namespace Snapdragon
{
    public record Location(
        Column Column,
        LocationDefinition Definition,
        ImmutableList<ICard> TopCardsIncludingUnrevealed,
        ImmutableList<ICard> BottomCardsIncludingUnrevealed,
        Multipliers TopMultipliers,
        Multipliers BottomMultipliers,
        ImmutableList<Sensor<ICard>> Sensors,
        bool Revealed = false
    ) : IObjectWithColumn
    {
        public Location(Column Column, LocationDefinition Definition)
            : this(Column, Definition, [], [], new(), new(), []) { }

        public MultipliersAccessor Multipliers
        {
            get { return new MultipliersAccessor(this); }
        }

        public ImmutableList<ICard> this[Side side]
        {
            get
            {
                switch (side)
                {
                    case Side.Top:
                        return TopCardsIncludingUnrevealed;
                    case Side.Bottom:
                        return BottomCardsIncludingUnrevealed;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IEnumerable<ICard> AllCardsIncludingUnrevealed =>
            this.TopCardsIncludingUnrevealed.Concat(this.BottomCardsIncludingUnrevealed);

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
                    return this with
                    {
                        TopCardsIncludingUnrevealed = this.TopCardsIncludingUnrevealed.Add(
                            cardInPlay
                        )
                    };
                case Side.Bottom:
                    return this with
                    {
                        BottomCardsIncludingUnrevealed = this.BottomCardsIncludingUnrevealed.Add(
                            cardInPlay
                        )
                    };
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
        public Location WithoutCard(ICardInstance card)
        {
            // TODO: Consider checking that the Card.State is correct
            switch (card.Side)
            {
                case Side.Top:
                    return this with
                    {
                        TopCardsIncludingUnrevealed = this.TopCardsIncludingUnrevealed.RemoveAll(
                            c => c.Id == card.Id
                        )
                    };
                case Side.Bottom:
                    return this with
                    {
                        BottomCardsIncludingUnrevealed =
                            this.BottomCardsIncludingUnrevealed.RemoveAll(c => c.Id == card.Id)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Removes a given <see cref="ICard"/> from the Location.
        ///
        /// Note that this does not apply any other game logic - it should be called
        /// by something that's orchestrating whatever is supposed to happen with the <see cref="Card"/>.
        ///
        /// Note also that the <see cref="ICard"/> is matched by Id to ensure nothing weird happens.
        /// </summary>
        public Location WithRemovedCard(ICard card)
        {
            // TODO: Consider checking that the Card.State is correct
            switch (card.Side)
            {
                case Side.Top:
                    return this with
                    {
                        TopCardsIncludingUnrevealed = this.TopCardsIncludingUnrevealed.RemoveAll(
                            c => c.Id == card.Id
                        )
                    };
                case Side.Bottom:
                    return this with
                    {
                        BottomCardsIncludingUnrevealed =
                            this.BottomCardsIncludingUnrevealed.RemoveAll(c => c.Id == card.Id)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public Location WithSensor(Sensor<ICard> sensor)
        {
            return this with { Sensors = this.Sensors.Add(sensor) };
        }

        public Location WithSensorDeleted(long sensorId)
        {
            return this with { Sensors = this.Sensors.RemoveAll(t => t.Id == sensorId) };
        }
    }
}
