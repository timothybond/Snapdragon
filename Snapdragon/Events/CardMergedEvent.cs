namespace Snapdragon.Events
{
    public record CardMergedEvent(int Turn, ICard Merged, ICard Target)
        : Event(EventType.CardMerged, Turn)
    {
        public override string ToString()
        {
            return $"Card Merged ({Merged.Side}): {Merged.Name} ({Merged.Id}) into {Target.Name} ({Target.Id})";
        }
    }
}
