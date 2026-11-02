using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Application.Services;

public sealed partial class VehiculoService : ServiceEntity<Vehiculo>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVehiculoRepository _repository;

    public VehiculoService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public VehiculoService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private VehiculoService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.VehiculoRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.VehiculoRepository;
    }

    public Task<Vehiculo> AddVehiculoDefaultAsync(Vehiculo entity) =>
        AddDefaultAsync(entity);

    public Task DeleteVehiculoDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Vehiculo?> ReadOIDVehiculoDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Vehiculo>> ReadAllVehiculoDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Vehiculo> ModifierVehiculoDefaultAsync(Vehiculo entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Vehiculo> NewAsync(string p_marcaModelo, string p_matricula, string p_informacionAdicional, int p_propietario_oid)
    {
        Usuario propietarioEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_propietario_oid)
            ?? throw new ModelException("Usuario not found for Vehiculo.Propietario.");

        Vehiculo entity = new Vehiculo
        {
            MarcaModelo = p_marcaModelo,
            Matricula = p_matricula,
            InformacionAdicional = p_informacionAdicional,
            Propietario = propietarioEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Vehiculo entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Vehiculo not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_marcaModelo, string p_matricula)
    {
        Vehiculo entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Vehiculo not found.");
        entity.MarcaModelo = p_marcaModelo;
        entity.Matricula = p_matricula;
        await _repository.SaveAsync(entity);
    }

    public async Task<Vehiculo?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Vehiculo>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
