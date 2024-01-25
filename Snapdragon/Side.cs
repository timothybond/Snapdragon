using System.Runtime.CompilerServices;

namespace Snapdragon
{
    public enum Side
    {
        Top = 0,
        Bottom = 1
    }

    public static class SideExtensions
    {
        public static Side OtherSide(this Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return Side.Bottom;
                case Side.Bottom:
                    return Side.Top;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
