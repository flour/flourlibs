namespace Flour.RabbitMQ;

public interface IConventionsStore
{
    void Add<T>(IMessageConvention convention);
    void Add(Type type, IMessageConvention convention);
    IMessageConvention Get<T>();
    IMessageConvention Get(Type type);
    IEnumerable<IMessageConvention> GetAll();
}