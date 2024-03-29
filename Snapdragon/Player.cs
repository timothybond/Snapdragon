namespace Snapdragon
{
    public record Player(PlayerConfiguration Configuration, Side Side, int Energy = 0)
    {
        public IPlayerController Controller => this.Configuration.Controller;

        /// <summary>
        /// Modifies the <see cref="IPlayerController"/> used to get actions for this Player.
        ///
        /// The main use of this is to allow us to put in different <see cref="IPlayerController"/>s as needed to simulate games.
        /// </summary>
        public Player WithController(IPlayerController controller)
        {
            return this with
            {
                Configuration = this.Configuration with { Controller = controller }
            };
        }
    }
}
