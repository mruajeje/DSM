using System.Threading.Tasks;
using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.Services;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;
using Moq;
using Xunit;

namespace CarGo.Tests.Manual;

public class ViajeServiceReservaTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReservaRepository> _reservaRepoMock;
    private readonly Mock<IViajeRepository> _viajeRepoMock;
    private readonly ViajeService _service;

    public ViajeServiceReservaTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _reservaRepoMock = new Mock<IReservaRepository>();
        _viajeRepoMock = new Mock<IViajeRepository>();

        _unitOfWorkMock.Setup(u => u.ReservaRepository).Returns(_reservaRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ViajeRepository).Returns(_viajeRepoMock.Object);

        _service = new ViajeService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RechazarPasajero_ConductorValidoYReservaPendiente_CambiaEstadoARechazada()
    {
        // Arrange
        int conductorId = 10;
        var viaje = new Viaje { Id = 1, Conductor = new Usuario { Id = conductorId } };
        var reserva = new Reserva { Id = 5, Estado = EstadoReserva.Pendiente, Viaje = viaje };

        _reservaRepoMock.Setup(r => r.ReadAsync(5)).ReturnsAsync(reserva);

        // Act
        await _service.Reserva_rechazarPasajeroAsync(5, conductorId);

        // Assert
        Assert.Equal(EstadoReserva.Rechazada, reserva.Estado);
        _reservaRepoMock.Verify(r => r.SaveAsync(reserva), Times.Once);
    }

    [Fact]
    public async Task RechazarPasajero_UsuarioNoEsConductor_LanzaModelException()
    {
        // Arrange
        var viaje = new Viaje { Id = 1, Conductor = new Usuario { Id = 10 } };
        var reserva = new Reserva { Id = 5, Estado = EstadoReserva.Pendiente, Viaje = viaje };

        _reservaRepoMock.Setup(r => r.ReadAsync(5)).ReturnsAsync(reserva);

        // Act & Assert
        await Assert.ThrowsAsync<ModelException>(() => _service.Reserva_rechazarPasajeroAsync(5, 99));
    }

    [Fact]
    public async Task CancelarReserva_ReservaAceptada_DevuelvePlazasYMarcaCancelada()
    {
        // Arrange
        int pasajeroId = 20;
        var viaje = new Viaje
        {
            Id = 1,
            NumPlazasDisponibles = 2,
            Conductor = new Usuario { Id = 10 }
        };
        var reserva = new Reserva
        {
            Id = 8,
            Estado = EstadoReserva.Aceptada,
            NumPlazasReservadas = 2,
            Pasajero = new Usuario { Id = pasajeroId },
            Viaje = viaje
        };

        _reservaRepoMock.Setup(r => r.ReadAsync(8)).ReturnsAsync(reserva);

        // Act
        await _service.Reserva_cancelarReservaAsync(8, pasajeroId);

        // Assert
        Assert.Equal(EstadoReserva.Cancelada, reserva.Estado);
        Assert.Equal(4, viaje.NumPlazasDisponibles); // 2 iniciales + 2 liberadas
        _viajeRepoMock.Verify(v => v.SaveAsync(viaje), Times.Once);
        _reservaRepoMock.Verify(r => r.SaveAsync(reserva), Times.Once);
    }
}