﻿namespace Snapdragon
{
    public interface ISensorTriggeredAbility
    {
        Game ProcessEvent(Game game, Event e, Sensor<Card> source);
    }
}
