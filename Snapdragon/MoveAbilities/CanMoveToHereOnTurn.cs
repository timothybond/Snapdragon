﻿namespace Snapdragon.MoveAbilities
{
    public record CanMoveToHereOnTurn<T>(int Turn) : IMoveAbility<T>
        where T : IObjectWithColumn
    {
        public bool CanMove(ICard target, T source, Column destination, Game game)
        {
            return game.Turn == Turn && destination == source.Column;
        }
    }
}
