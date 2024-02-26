namespace Snapdragon
{
    public class RepositoryGameLogger : IGameLogger
    {
        private readonly ISnapdragonRepository _repository;
        private readonly Guid _gameId;
        private int _order = 0;

        public RepositoryGameLogger(ISnapdragonRepository repository, Guid gameId)
        {
            _repository = repository;
            _gameId = gameId;
        }

        public async Task LogEvent(Event e)
        {
            var record = new GameLogRecord(_gameId, _order, e.Turn, e.ToString());
            _order++;

            await _repository.SaveGameLog(record);
        }

        public Task LogFinishedGame(Game game)
        {
            return Task.CompletedTask;
        }

        public async Task LogGameState(Game game)
        {
            var record = new GameLogRecord(
                _gameId,
                _order,
                game.Turn,
                LoggerUtilities.GameStateLog(game)
            );
            _order++;

            await _repository.SaveGameLog(record);
        }

        public async Task LogHands(Game game)
        {
            var record = new GameLogRecord(
                _gameId,
                _order,
                game.Turn,
                LoggerUtilities.HandsLog(game)
            );
            _order++;

            await _repository.SaveGameLog(record);
        }
    }
}
