namespace Flour.CQRS;

public interface IQueryHandler<in Q, T> where Q : class, IQuery<T>
{
    Task<T> Handle(Q query);
}