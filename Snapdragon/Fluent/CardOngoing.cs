using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public record CardOngoing : OngoingBuilder<Card>
    {
        public CardOngoing()
            : base() { }
    }
}
