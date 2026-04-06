using System.Linq.Expressions;

namespace NotesKeeper.Infrastructure.Repositories
{
    internal static class ExpressionConverter
    {
        /// <summary>
        /// Converts an <see cref="Expression{Predicate{T}}"/> to an <see cref="Expression{Func{T, Boolean}}"/>
        /// so it can be used with EF Core LINQ methods.
        /// </summary>
        public static Expression<Func<T, bool>> ToFuncExpression<T>(Expression<Predicate<T>> predicate)
        {
            return Expression.Lambda<Func<T, bool>>(predicate.Body, predicate.Parameters);
        }
    }
}
