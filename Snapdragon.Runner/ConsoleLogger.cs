﻿namespace Snapdragon.Runner
{
    internal class ConsoleLogger : IGameLogger
    {
        public void LogEvent(Event e)
        {
            Console.WriteLine(e.ToString());
        }

        public void LogGameState(GameState game)
        {
            // TODO: Decide how to log game state
            return;
        }
    }
}