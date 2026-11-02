namespace CarGoGen.Application.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IUsuarioRepository UsuarioRepository { get; }
    IVehiculoRepository VehiculoRepository { get; }
    IViajeRepository ViajeRepository { get; }
    IReservaRepository ReservaRepository { get; }
    IPagoRepository PagoRepository { get; }
    IMensajeRepository MensajeRepository { get; }
    IResenaRepository ResenaRepository { get; }
    INotificacionRepository NotificacionRepository { get; }

    IHariaPasswordHasher PasswordHasher { get; }
    IHariaTokenGenerator TokenGenerator { get; }

    Task BeginTransactionAsync(string? businessOperation = null);
    Task CommitAsync();
    Task RollbackAsync();
}
