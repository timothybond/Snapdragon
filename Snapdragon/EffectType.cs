namespace Snapdragon
{
    /// <summary>
    /// Categories of effects. Primarily used for cards/locations that prevent certain effects from happening.
    /// </summary>
    public enum EffectType
    {
        PlayCard,
        DestroyCard,
        MoveCard,
        AddCard, // Technically a superset of "PlayCard" and "MoveCard"

        // These two are location-specific
        MoveToLocation,
        MoveFromLocation,

        ReducePower, // Technically a subset of "AdjustPower", but some abilities mention this specifically

        // These two are player-specific
        IncreaseCost,
        ReduceCost,

        OnRevealAbilities,
        OngoingAbilities
    }
}
