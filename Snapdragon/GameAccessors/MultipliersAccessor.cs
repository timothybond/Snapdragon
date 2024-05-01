namespace Snapdragon.GameAccessors
{
    public class MultipliersAccessor
    {
        private Location location;

        public MultipliersAccessor(Location location)
        {
            this.location = location;
        }

        public Multipliers this[Side side]
        {
            get
            {
                switch (side)
                {
                    case Side.Top:
                        return location.TopMultipliers;
                    case Side.Bottom:
                        return location.BottomMultipliers;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
