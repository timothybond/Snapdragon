namespace Snapdragon.GameKernelAccessors
{
    public record TopPlayerAccessor(Game Game, Player Player) : IPlayerAccessor
    {
        public IReadOnlyList<ICardInstance> Hand => new CardListReference(Game.Kernel, Game.Kernel.TopHand);

        public IReadOnlyList<ICardInstance> Library =>
            new CardListReference(Game.Kernel, Game.Kernel.TopLibrary);

        public IReadOnlyList<ICardInstance> Discards =>
            new CardListReference(Game.Kernel, Game.Kernel.TopDiscards);

        public IReadOnlyList<ICardInstance> Destroyed =>
            new CardListReference(Game.Kernel, Game.Kernel.TopDestroyed);

        public Side Side => Player.Side;

        public PlayerConfiguration Configuration => Player.Configuration;

        public IPlayerController Controller => Configuration.Controller;

        public int Energy => Game.TopPlayer.Energy;
    }
}
