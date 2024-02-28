namespace Snapdragon.Api.Data
{
    public class Game
    {
        public Guid Id { get; set; }
        public Guid TopPlayerId { get; set; }
        public Guid BottomPlayerId { get; set; }
        public required string Winner { get; set; }
        public Guid? ExperimentId { get; set; }
        public int? Generation { get; set; }
        public List<GameLog>? Logs { get; set; }

        public static explicit operator Game(GameRecord gr) =>
            new Game
            {
                Id = gr.GameId,
                TopPlayerId = gr.TopPlayerId,
                BottomPlayerId = gr.BottomPlayerId,
                Winner = gr.Winner?.ToString() ?? "",
                ExperimentId = gr.ExperimentId,
                Generation = gr.Generation
            };
    }
}
