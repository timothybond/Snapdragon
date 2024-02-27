namespace Snapdragon
{
    public class RepositoryGameLogger : IGameLogger
    {
        private readonly ISnapdragonRepository _repository;
        private readonly Guid _gameId;
        private readonly Guid? _experimentId;
        private readonly int? _generation;
        private int _order = 0;
        private readonly List<GameLogRecord> _logs = new List<GameLogRecord>();

        public RepositoryGameLogger(
            ISnapdragonRepository repository,
            Guid gameId,
            Guid? experimentId,
            int? generation
        )
        {
            _repository = repository;
            _gameId = gameId;
            _experimentId = experimentId;
            _generation = generation;
        }

        public void LogEvent(Event e)
        {
            var log = new GameLogRecord(_gameId, _order, e.Turn, e.ToString());
            _order++;
            _logs.Add(log);
        }

        public async Task LogFinishedGame(Game game)
        {
            var winner = game.GetLeader();
            try
            {
                var retries = 1;

                while (retries >= 0)
                {
                    try
                    {
                        await _repository.SaveGame(
                            new GameRecord(
                                game.Id,
                                game.Top.Configuration.Deck.Id,
                                game.Bottom.Configuration.Deck.Id,
                                winner,
                                _experimentId,
                                _generation
                            )
                        );

                        foreach (var log in _logs)
                        {
                            await _repository.SaveGameLog(log);
                        }

                        return;
                    }
                    catch (Exception)
                    {
                        retries--;
                        if (retries < 0)
                        {
                            throw;
                        }

                        await _repository.DeleteGame(game.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save full log for game {game.Id}:\n{ex}");
            }
        }

        public void LogGameState(Game game)
        {
            var log = new GameLogRecord(
                _gameId,
                _order,
                game.Turn,
                LoggerUtilities.GameStateLog(game)
            );
            _order++;
            _logs.Add(log);
        }

        public void LogHands(Game game)
        {
            var log = new GameLogRecord(_gameId, _order, game.Turn, LoggerUtilities.HandsLog(game));
            _order++;
            _logs.Add(log);
        }
    }
}
