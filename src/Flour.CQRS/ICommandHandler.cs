namespace Flour.CQRS;

public interface ICommandHandler<in T> where T : class, ICommand
{
    Task Handle(T command);
}