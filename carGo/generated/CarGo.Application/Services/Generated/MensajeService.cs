using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

namespace CarGo.Application.Services;

public sealed partial class MensajeService : ServiceEntity<Mensaje>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMensajeRepository _repository;

    public MensajeService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public MensajeService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private MensajeService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.MensajeRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.MensajeRepository;
    }

    public Task<Mensaje> AddMensajeDefaultAsync(Mensaje entity) =>
        AddDefaultAsync(entity);

    public Task DeleteMensajeDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Mensaje?> ReadOIDMensajeDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Mensaje>> ReadAllMensajeDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Mensaje> ModifierMensajeDefaultAsync(Mensaje entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Mensaje> NewAsync(string p_contenido, DateTime p_fechaHora, int p_viaje_oid, int p_remitente_oid)
    {
        Viaje viajeEntity = await _unitOfWork.ViajeRepository.ReadAsync(p_viaje_oid)
            ?? throw new ModelException("Viaje not found for Mensaje.Viaje.");

        Usuario remitenteEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_remitente_oid)
            ?? throw new ModelException("Usuario not found for Mensaje.Remitente.");

        Mensaje entity = new Mensaje
        {
            Contenido = p_contenido,
            FechaHora = p_fechaHora,
            Viaje = viajeEntity,
            Remitente = remitenteEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Mensaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Mensaje not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_contenido, DateTime p_fechaHora)
    {
        Mensaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Mensaje not found.");
        entity.Contenido = p_contenido;
        entity.FechaHora = p_fechaHora;
        await _repository.SaveAsync(entity);
    }

    public async Task<Mensaje?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Mensaje>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
