using Snapdragon.Fluent.Filters;

namespace Snapdragon.Fluent
{
    public static class Moved
    {
        public static MovedToHere ToHere => new MovedToHere();

        public static MovedFromHere FromHere => new MovedFromHere();
    }
}
