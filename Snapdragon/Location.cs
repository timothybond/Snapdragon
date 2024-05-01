using Snapdragon.GameAccessors;

namespace Snapdragon
{
    /// The combination of a <see cref="LocationDefinition"/>, <see cref="Column"/> and a reference to a
    /// <see cref="GameKernel"/> that allows us to retrieve the ephemeral attributes from that <see cref="GameKernel"/>.
    ///
    /// Note that because <see cref="GameKernel"/> can only be altered
    /// by creating new instances, the values of this object are unreliable
    /// whenever anything changes.
    ///
    /// However, it is by design fairly low effort to create a new one of these after any change.
    public record Location(
        Column Column,
        LocationDefinition Definition,
        GameKernel Kernel,
        Multipliers TopMultipliers,
        Multipliers BottomMultipliers
    ) : IObjectWithColumn
    {
        public MultipliersAccessor Multipliers
        {
            get { return new MultipliersAccessor(this); }
        }

        public IReadOnlyList<ICard> this[Side side]
        {
            get
            {
                switch (side)
                {
                    case Side.Top:
                        return TopCards;
                    case Side.Bottom:
                        return BottomCards;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public bool Revealed
        {
            get
            {
                switch (Column)
                {
                    case Column.Left:
                        return Kernel.LeftRevealed;
                    case Column.Middle:
                        return Kernel.MiddleRevealed;
                    case Column.Right:
                        return Kernel.RightRevealed;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets the cards for the top Player, in this location.
        ///
        /// Note that this includes unrevealed cards, out of what might be a misguided attempt to keep things efficient.
        /// </summary>
        public IReadOnlyList<ICard> TopCards => Kernel[Column, Side.Top];

        /// <summary>
        /// Gets the cards for the bottom Player, in this location.
        ///
        /// Note that this includes unrevealed cards, out of what might be a misguided attempt to keep things efficient.
        /// </summary>
        public IReadOnlyList<ICard> BottomCards => Kernel[Column, Side.Bottom];

        /// <summary>
        /// Gets all cards in the location.
        ///
        /// Note that this includes unrevealed cards, for consistency.
        /// </summary>
        public IEnumerable<ICard> AllCards => TopCards.Concat(BottomCards);

        public IEnumerable<Sensor<ICard>> TopSensors
        {
            get
            {
                switch (Column)
                {
                    case Column.Left:
                        return Kernel.TopLeftSensors.Select(id => Kernel.Sensors[id]);
                    case Column.Middle:
                        return Kernel.TopMiddleSensors.Select(id => Kernel.Sensors[id]);
                    case Column.Right:
                        return Kernel.TopRightSensors.Select(id => Kernel.Sensors[id]);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IEnumerable<Sensor<ICard>> BottomSensors
        {
            get
            {
                switch (Column)
                {
                    case Column.Left:
                        return Kernel.BottomLeftSensors.Select(id => Kernel.Sensors[id]);
                    case Column.Middle:
                        return Kernel.BottomMiddleSensors.Select(id => Kernel.Sensors[id]);
                    case Column.Right:
                        return Kernel.BottomRightSensors.Select(id => Kernel.Sensors[id]);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IEnumerable<Sensor<ICard>> AllSensors => TopSensors.Concat(BottomSensors);

        Column? IObjectWithPossibleColumn.Column => Column;
    }
}
