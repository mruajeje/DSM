using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Application.Services;

public sealed partial class NotificacionService : ServiceEntity<Notificacion>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificacionRepository _repository;

    public NotificacionService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public NotificacionService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private NotificacionService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.NotificacionRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.NotificacionRepository;
    }

    public Task<Notificacion> AddNotificacionDefaultAsync(Notificacion entity) =>
        AddDefaultAsync(entity);

    public Task DeleteNotificacionDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Notificacion?> ReadOIDNotificacionDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Notificacion>> ReadAllNotificacionDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Notificacion> ModifierNotificacionDefaultAsync(Notificacion entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Notificacion> NewAsync(string p_titulo, string p_texto, DateTime p_fechaHora, bool p_leida, int p_usuarioNotificado_oid)
    {
        Usuario usuarioNotificadoEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuarioNotificado_oid)
            ?? throw new ModelException("Usuario not found for Notificacion.UsuarioNotificado.");

        Notificacion entity = new Notificacion
        {
            Titulo = p_titulo,
            Texto = p_texto,
            FechaHora = p_fechaHora,
            Leida = p_leida,
            UsuarioNotificado = usuarioNotificadoEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Notificacion entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Notificacion not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_titulo, string p_texto, DateTime p_fechaHora, bool p_leida, int p_usuarioNotificado_oid)
    {
        Notificacion entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Notificacion not found.");
        entity.Titulo = p_titulo;
        entity.Texto = p_texto;
        entity.FechaHora = p_fechaHora;
        entity.Leida = p_leida;
        await _repository.SaveAsync(entity);
    }

    public async Task<Notificacion?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Notificacion>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
