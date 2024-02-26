namespace Snapdragon.Postgresql.Data
{
    public class GameLog
    {
        public Guid GameId { get; set; }

        public int LogOrder { get; set; }

        public int Turn { get; set; }

        public required string Contents { get; set; }

        public static explicit operator GameLog(GameLogRecord glr) =>
            new GameLog
            {
                GameId = glr.GameId,
                LogOrder = glr.Order,
                Turn = glr.Turn,
                Contents = glr.Contents
            };

        public static explicit operator GameLogRecord(GameLog gl) =>
            new GameLogRecord(gl.GameId, gl.LogOrder, gl.Turn, gl.Contents);
    }
}
