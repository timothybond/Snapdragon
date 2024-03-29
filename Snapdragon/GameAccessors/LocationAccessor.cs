namespace Snapdragon.GameKernelAccessors
{
    public record LocationAccessor(Game Game, Column Column)
    {
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
                        return Game.Kernel.LeftRevealed;
                    case Column.Middle:
                        return Game.Kernel.MiddleRevealed;
                    case Column.Right:
                        return Game.Kernel.RightRevealed;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public LocationDefinition Definition
        {
            get
            {
                switch (Column)
                {
                    case Column.Left:
                        return Game.Left.Definition;
                    case Column.Middle:
                        return Game.Middle.Definition;
                    case Column.Right:
                        return Game.Right.Definition;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IReadOnlyList<ICard> TopCards => Game.Kernel[Column, Side.Top];

        public IReadOnlyList<ICard> BottomCards => Game.Kernel[Column, Side.Bottom];

        public IEnumerable<ICard> AllCards => TopCards.Concat(BottomCards);

        public IEnumerable<Sensor<ICard>> TopSensors
        {
            get
            {
                switch (Column)
                {
                    case Column.Left:
                        return Game.Kernel.TopLeftSensors.Select(id => Game.Kernel.Sensors[id]);
                    case Column.Middle:
                        return Game.Kernel.TopMiddleSensors.Select(id => Game.Kernel.Sensors[id]);
                    case Column.Right:
                        return Game.Kernel.TopRightSensors.Select(id => Game.Kernel.Sensors[id]);
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
                        return Game.Kernel.BottomLeftSensors.Select(id => Game.Kernel.Sensors[id]);
                    case Column.Middle:
                        return Game.Kernel.BottomMiddleSensors.Select(id =>
                            Game.Kernel.Sensors[id]
                        );
                    case Column.Right:
                        return Game.Kernel.BottomRightSensors.Select(id => Game.Kernel.Sensors[id]);
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public IEnumerable<Sensor<ICard>> Sensors => TopSensors.Concat(BottomSensors);
    }
}
