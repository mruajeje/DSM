namespace CarGoGen.Application.Abstractions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}