using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

namespace CarGo.Application.Services;

public sealed partial class ResenaService : ServiceEntity<Resena>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResenaRepository _repository;

    public ResenaService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public ResenaService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private ResenaService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.ResenaRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.ResenaRepository;
    }

    public Task<Resena> AddResenaDefaultAsync(Resena entity) =>
        AddDefaultAsync(entity);

    public Task DeleteResenaDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Resena?> ReadOIDResenaDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Resena>> ReadAllResenaDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Resena> ModifierResenaDefaultAsync(Resena entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Resena> NewAsync(int p_puntuacion, string p_comentario, DateTime p_fechaCreacion, EstadoResena p_estado, int p_viaje_oid, int p_autor_oid)
    {
        Viaje viajeEntity = await _unitOfWork.ViajeRepository.ReadAsync(p_viaje_oid)
            ?? throw new ModelException("Viaje not found for Resena.Viaje.");

        Usuario autorEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_autor_oid)
            ?? throw new ModelException("Usuario not found for Resena.Autor.");

        Resena entity = new Resena
        {
            Puntuacion = p_puntuacion,
            Comentario = p_comentario,
            FechaCreacion = p_fechaCreacion,
            Estado = p_estado,
            Viaje = viajeEntity,
            Autor = autorEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Resena entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Resena not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, int p_puntuacion, DateTime p_fechaCreacion, EstadoResena p_estado)
    {
        Resena entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Resena not found.");
        entity.Puntuacion = p_puntuacion;
        entity.FechaCreacion = p_fechaCreacion;
        entity.Estado = p_estado;
        await _repository.SaveAsync(entity);
    }

    public async Task<Resena?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Resena>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
