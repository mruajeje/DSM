namespace CarGo.Application.Abstractions;

public interface IUnitOfWorkUtils
{
    IServiceProvider Services { get; }

    T GetRequiredService<T>() where T : notnull;

    T? GetService<T>();
}