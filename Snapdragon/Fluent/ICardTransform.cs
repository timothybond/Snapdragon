namespace Snapdragon.Fluent
{
    /// <summary>
    /// Performs a modification to an <see cref="ICardInstance"/>.
    ///
    /// Because of some quirks of the current types, this will transform it to a
    /// <see cref="CardInstance"/>, but this should be safe as anybody using it
    /// as a <see cref="Card"/> will need to force it back into that state.
    /// </summary>
    public interface ICardTransform
    {
        ICardInstance Apply(ICardInstance card);
    }
}
