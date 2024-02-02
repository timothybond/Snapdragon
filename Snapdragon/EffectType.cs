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

        // These two are location-specific
        MoveToLocation,
        MoveFromLocation,

        AdjustPower,
        ReducePower // Technically a subset of "AdjustPower", but some abilities mention this specifically
    }
}
