using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public record CardOngoing : OngoingBuilder<ICard>
    {
        public CardOngoing()
            : base() { }
    }
}
