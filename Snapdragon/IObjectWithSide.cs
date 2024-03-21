namespace Snapdragon
{
    public interface IObjectWithSide : IObjectWithPossibleColumn
    {
        // Note: Making IObjectWithSide implement IObjectWithPossibleColumn is arguably
        // not the best choice, because theoretically something could have a Side
        // without ever having a Column, but it does clear up what would otherwise
        // be some issues in the call chain for some of the fluent ability builders.

        Side Side { get; }
    }
}
