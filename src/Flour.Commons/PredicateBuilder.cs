using System.Linq.Expressions;

namespace Flour.Commons;

public static class PredicateBuilder
{
    extension<T>(Expression<Func<T, bool>> a)
    {
        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> b)
        {
            var p = a.Parameters[0];
            var visitor = new SubstExpressionVisitor
            {
                Subst =
                {
                    [b.Parameters[0]] = p
                }
            };

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> b)
        {
            var p = a.Parameters[0];
            var visitor = new SubstExpressionVisitor
            {
                Subst =
                {
                    [b.Parameters[0]] = p
                }
            };
            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}

internal class SubstExpressionVisitor : ExpressionVisitor
{
    public readonly Dictionary<Expression, Expression> Subst = new();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Subst.GetValueOrDefault(node, node);
    }
}