namespace Snapdragon.GameKernelAccessors
{
    public record BottomPlayerAccessor(Game Game, Player Player) : IPlayerAccessor
    {
        public IReadOnlyList<ICardInstance> Hand =>
            new CardListReference(Game.Kernel, Game.Kernel.BottomHand);

        public IReadOnlyList<ICardInstance> Library =>
            new CardListReference(Game.Kernel, Game.Kernel.BottomLibrary);

        public IReadOnlyList<ICardInstance> Discards =>
            new CardListReference(Game.Kernel, Game.Kernel.BottomDiscards);

        public IReadOnlyList<ICardInstance> Destroyed =>
            new CardListReference(Game.Kernel, Game.Kernel.BottomDestroyed);

        public Side Side => Player.Side;

        public PlayerConfiguration Configuration => Player.Configuration;

        public IPlayerController Controller => Configuration.Controller;

        public int Energy => Game.BottomPlayer.Energy;
    }
}
