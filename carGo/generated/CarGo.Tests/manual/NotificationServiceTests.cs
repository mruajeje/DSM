using System.Threading.Tasks;
using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Application.Services;
using CarGo.Domain.Entities;
using Moq;
using Xunit;

namespace CarGo.Tests;

public class NotificationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<INotificacionRepository> _repoMock;
    private readonly NotificacionService _service;

    public NotificationServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repoMock = new Mock<INotificacionRepository>();

        // Verknüpfung vum Repository am UnitOfWork Mock
        _unitOfWorkMock.Setup(u => u.NotificacionRepository).Returns(_repoMock.Object);

        // NotificacionService instantiéieren mat IUnitOfWork
        _service = new NotificacionService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task MarcarComoLeidaAsync_NotificacionValida_MarcaComoLeidaYGuarda()
    {
        // Arrange
        int notifId = 1;
        int usuarioId = 5;
        var usuario = new Usuario { Id = usuarioId };
        var notificacion = new Notificacion
        {
            Id = notifId,
            UsuarioNotificado = usuario,
            Leida = false
        };

        _repoMock.Setup(r => r.ReadAsync(notifId)).ReturnsAsync(notificacion);

        // Act
        await _service.MarcarComoLeidaAsync(notifId, usuarioId);

        // Assert
        Assert.True(notificacion.Leida);
        _repoMock.Verify(r => r.SaveAsync(notificacion), Times.Once);
    }

    [Fact]
    public async Task MarcarComoLeidaAsync_NotificacionInexistente_LanzaModelException()
    {
        // Arrange
        _repoMock.Setup(r => r.ReadAsync(It.IsAny<int>())).ReturnsAsync((Notificacion?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ModelException>(() => _service.MarcarComoLeidaAsync(99, 5));
    }

    [Fact]
    public async Task MarcarComoLeidaAsync_UsuarioDiferente_LanzaModelException()
    {
        // Arrange
        var usuarioPropietario = new Usuario { Id = 10 };
        var notificacion = new Notificacion
        {
            Id = 1,
            UsuarioNotificado = usuarioPropietario,
            Leida = false
        };

        _repoMock.Setup(r => r.ReadAsync(1)).ReturnsAsync(notificacion);

        // Act & Assert
        await Assert.ThrowsAsync<ModelException>(() => _service.MarcarComoLeidaAsync(1, 999));
    }
}