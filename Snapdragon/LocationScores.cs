namespace Snapdragon
{
    public record LocationScores(Column Column, int Top, int Bottom)
    {
        public Side? Leader
        {
            get
            {
                if (this.Top > this.Bottom)
                {
                    return Side.Top;
                }
                else if (this.Top < this.Bottom)
                {
                    return Side.Bottom;
                }
                else
                {
                    return null;
                }
            }
        }

        public int this[Side side]
        {
            get
            {
                switch (side)
                {
                    case Side.Top:
                        return this.Top;
                    case Side.Bottom:
                        return this.Bottom;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public LocationScores WithAddedPower(int amount, Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return this with { Top = this.Top + amount };
                case Side.Bottom:
                    return this with { Bottom = this.Bottom + amount };
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
