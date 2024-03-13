namespace Snapdragon
{
    public interface IObjectWithPossibleColumn
    {
        Column? Column { get; }
    }

    public interface IObjectWithColumn : IObjectWithPossibleColumn
    {
        new Column Column { get; }
    }
}
