namespace Snapdragon.Postgresql.Data
{
    public class Game
    {
        public Guid Id { get; set; }

        public Guid TopItem { get; set; }

        public Guid BottomItem { get; set; }
    }
}
