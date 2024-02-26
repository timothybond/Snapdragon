namespace Snapdragon.Postgresql.Data
{
    public class GameRecord
    {
        public Guid Id { get; set; }

        public Guid TopItemId { get; set; }

        public Guid BottomItemId { get; set; }

        public string? Winner { get; set; }

        public Guid? ExperimentId { get; set; }

        public int? Generation { get; set; }

        public static explicit operator Snapdragon.GameRecord(GameRecord gr)
        {
            Side? winner = gr.Winner == null ? null : Enum.Parse<Side>(gr.Winner);

            return new Snapdragon.GameRecord(
                gr.Id,
                gr.TopItemId,
                gr.BottomItemId,
                winner,
                gr.ExperimentId,
                gr.Generation
            );
        }

        public static explicit operator GameRecord(Snapdragon.GameRecord gr) =>
            new GameRecord
            {
                Id = gr.GameId,
                TopItemId = gr.TopPlayerId,
                BottomItemId = gr.BottomPlayerId,
                Winner = gr.Winner?.ToString(),
                ExperimentId = gr.ExperimentId,
                Generation = gr.Generation
            };
    }
}
