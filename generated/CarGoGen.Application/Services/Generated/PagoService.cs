using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Application.Services;

public sealed partial class PagoService : ServiceEntity<Pago>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPagoRepository _repository;

    public PagoService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public PagoService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private PagoService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.PagoRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.PagoRepository;
    }

    public Task<Pago> AddPagoDefaultAsync(Pago entity) =>
        AddDefaultAsync(entity);

    public Task DeletePagoDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Pago?> ReadOIDPagoDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Pago>> ReadAllPagoDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Pago> ModifierPagoDefaultAsync(Pago entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Pago> NewAsync(Dinero p_monto, DateTime p_fechaPago, EstadoPago p_estado, int p_reserva_oid)
    {
        Reserva reservaEntity = await _unitOfWork.ReservaRepository.ReadAsync(p_reserva_oid)
            ?? throw new ModelException("Reserva not found for Pago.Reserva.");

        Pago entity = new Pago
        {
            Monto = p_monto,
            FechaPago = p_fechaPago,
            Estado = p_estado,
            Reserva = reservaEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Pago entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Pago not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, Dinero p_monto, DateTime p_fechaPago, EstadoPago p_estado)
    {
        Pago entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Pago not found.");
        entity.Monto = p_monto;
        entity.FechaPago = p_fechaPago;
        entity.Estado = p_estado;
        await _repository.SaveAsync(entity);
    }

    public async Task<Pago?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Pago>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
