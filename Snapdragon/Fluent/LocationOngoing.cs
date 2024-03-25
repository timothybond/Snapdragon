using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public record LocationOngoing : OngoingBuilder<Location>
    {
        public LocationOngoing()
            : base() { }
    }
}
