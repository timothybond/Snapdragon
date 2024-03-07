namespace Snapdragon.Api.Data
{
    public class GameLog
    {
        public int Order { get; set; }
        public int Turn { get; set; }
        public required string Contents { get; set; }

        public static explicit operator GameLog(GameLogRecord glr) =>
            new GameLog
            {
                Turn = glr.Turn,
                Contents = glr.Contents,
                Order = glr.Order
            };
    }
}
