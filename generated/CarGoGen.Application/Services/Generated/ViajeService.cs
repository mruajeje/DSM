using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Application.Services;

public sealed partial class ViajeService : ServiceEntity<Viaje>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IViajeRepository _repository;

    public ViajeService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public ViajeService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private ViajeService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.ViajeRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.ViajeRepository;
    }

    public Task<Viaje> AddViajeDefaultAsync(Viaje entity) =>
        AddDefaultAsync(entity);

    public Task DeleteViajeDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Viaje?> ReadOIDViajeDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Viaje>> ReadAllViajeDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Viaje> ModifierViajeDefaultAsync(Viaje entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Viaje> NewAsync(Ubicacion p_origen, Ubicacion p_destino, DateTime p_fechaHoraSalida, int p_numPlazasDisponibles, Dinero p_precio, bool p_permiteMascotas, bool p_permiteEquipaje, EstadoViaje p_estado, int p_conductor_oid)
    {
        Usuario conductorEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_conductor_oid)
            ?? throw new ModelException("Usuario not found for Viaje.Conductor.");

        Viaje entity = new Viaje
        {
            Origen = p_origen,
            Destino = p_destino,
            FechaHoraSalida = p_fechaHoraSalida,
            NumPlazasDisponibles = p_numPlazasDisponibles,
            Precio = p_precio,
            PermiteMascotas = p_permiteMascotas,
            PermiteEquipaje = p_permiteEquipaje,
            Estado = p_estado,
            Conductor = conductorEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Viaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, Ubicacion p_origen, Ubicacion p_destino, DateTime p_fechaHoraSalida, int p_numPlazasDisponibles, Dinero p_precio, bool p_permiteMascotas, bool p_permiteEquipaje, EstadoViaje p_estado)
    {
        Viaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");
        entity.Origen = p_origen;
        entity.Destino = p_destino;
        entity.FechaHoraSalida = p_fechaHoraSalida;
        entity.NumPlazasDisponibles = p_numPlazasDisponibles;
        entity.Precio = p_precio;
        entity.PermiteMascotas = p_permiteMascotas;
        entity.PermiteEquipaje = p_permiteEquipaje;
        entity.Estado = p_estado;
        await _repository.SaveAsync(entity);
    }

    public async Task<Viaje?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Viaje>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

    public async Task AddPasajeroAsync(int p_oid, IReadOnlyList<int> p_usuario_oids)
    {
        Viaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");
        entity.Pasajero ??= new List<Usuario>();
        foreach (var relatedId in p_usuario_oids)
        {
            Usuario relatedEntity = await _unitOfWork.UsuarioRepository.ReadAsync(relatedId)
                ?? throw new ModelException("Usuario not found for Viaje.Pasajero.");
            if (!entity.Pasajero.Contains(relatedEntity))
            {
                entity.Pasajero.Add(relatedEntity);
            }
            relatedEntity.Viajespasajero ??= new List<Viaje>();
            if (!relatedEntity.Viajespasajero.Contains(entity))
            {
                relatedEntity.Viajespasajero.Add(entity);
            }
            await _unitOfWork.UsuarioRepository.SaveAsync(relatedEntity);
        }
        await _repository.SaveAsync(entity);
    }

    public async Task RemovePasajeroAsync(int p_oid, IReadOnlyList<int> p_usuario_oids)
    {
        Viaje entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Viaje not found.");
        entity.Pasajero ??= new List<Usuario>();
        foreach (var relatedId in p_usuario_oids)
        {
            Usuario relatedEntity = await _unitOfWork.UsuarioRepository.ReadAsync(relatedId)
                ?? throw new ModelException("Usuario not found for Viaje.Pasajero.");
            if (!entity.Pasajero.Contains(relatedEntity))
            {
                entity.Pasajero.Add(relatedEntity);
            }
            relatedEntity.Viajespasajero ??= new List<Viaje>();
            if (!relatedEntity.Viajespasajero.Contains(entity))
            {
                relatedEntity.Viajespasajero.Add(entity);
            }
            await _unitOfWork.UsuarioRepository.SaveAsync(relatedEntity);
        }
        await _repository.SaveAsync(entity);
    }

    public async Task<Reserva> Reserva_newAsync(int p_numPlazasReservadas, EstadoReserva p_estado, DateTime p_fechaSolicitud, int p_viaje_oid, int p_pasajero_oid)
    {
        Viaje viajeEntity = await _unitOfWork.ViajeRepository.ReadAsync(p_viaje_oid)
            ?? throw new ModelException("Viaje not found for Reserva.Viaje.");

        Usuario pasajeroEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_pasajero_oid)
            ?? throw new ModelException("Usuario not found for Reserva.Pasajero.");

        Reserva entity = new Reserva
        {
            NumPlazasReservadas = p_numPlazasReservadas,
            Estado = p_estado,
            FechaSolicitud = p_fechaSolicitud,
            Viaje = viajeEntity,
            Pasajero = pasajeroEntity,
        };

        await _unitOfWork.ReservaRepository.SaveAsync(entity);
        return entity;
    }

    public async Task Reserva_destroyAsync(int p_oid)
    {
        Reserva entity = await _unitOfWork.ReservaRepository.ReadAsync(p_oid)
            ?? throw new ModelException("Reserva not found.");
        await _unitOfWork.ReservaRepository.DeleteAsync(entity);
    }

    public async Task Reserva_modifyAsync(int p_oid, int p_numPlazasReservadas, EstadoReserva p_estado, DateTime p_fechaSolicitud)
    {
        Reserva entity = await _unitOfWork.ReservaRepository.ReadAsync(p_oid)
            ?? throw new ModelException("Reserva not found.");
        entity.NumPlazasReservadas = p_numPlazasReservadas;
        entity.Estado = p_estado;
        entity.FechaSolicitud = p_fechaSolicitud;
        await _unitOfWork.ReservaRepository.SaveAsync(entity);
    }

    public async Task<Reserva?> Reserva_readOIDAsync(int p_oid)
    {
        return await _unitOfWork.ReservaRepository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Reserva>> Reserva_readAllAsync()
    {
        return await _unitOfWork.ReservaRepository.ReadAllAsync();
    }

}
