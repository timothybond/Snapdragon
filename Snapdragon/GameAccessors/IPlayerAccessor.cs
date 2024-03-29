namespace Snapdragon.GameKernelAccessors
{
    public interface IPlayerAccessor
    {
        IReadOnlyList<ICardInstance> Hand { get; }
        IReadOnlyList<ICardInstance> Library { get; }
        IReadOnlyList<ICardInstance> Discards { get; }
        IReadOnlyList<ICardInstance> Destroyed { get; }
        Side Side { get; }
        PlayerConfiguration Configuration { get; }
        IPlayerController Controller { get; }
        int Energy { get; }
    }
}
