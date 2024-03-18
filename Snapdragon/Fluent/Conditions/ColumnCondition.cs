using System.Collections.Immutable;

namespace Snapdragon.Fluent.Conditions
{
    public record ColumnCondition<TContext>(ImmutableList<Column> Columns) : ICondition<TContext>
        where TContext : IObjectWithColumn
    {
        public ColumnCondition(params Column[] Columns)
            : this(Columns.ToImmutableList()) { }

        public bool IsMet(TContext context, Game game)
        {
            return Columns.Contains(context.Column);
        }
    }
}
