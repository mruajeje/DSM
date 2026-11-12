namespace CarGo.Application.Abstractions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}