using System;
using System.Collections.Generic;
using System.Linq;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;
using Xunit;

namespace CarGoGen.Tests.Manual;

public class ReservaFilterTests
{
    [Fact]
    public void ReadFilterByEstadoYFecha_FiltraPorEstadoYRangoFechas()
    {
        // Arrange
        var desde = new DateTime(2026, 1, 1);
        var hasta = new DateTime(2026, 12, 31);
        var reservas = new List<Reserva>
        {
            new Reserva { Id = 1, Estado = EstadoReserva.Pendiente, FechaSolicitud = new DateTime(2026, 6, 15) },
            new Reserva { Id = 2, Estado = EstadoReserva.Aceptada, FechaSolicitud = new DateTime(2026, 6, 15) },
            new Reserva { Id = 3, Estado = EstadoReserva.Pendiente, FechaSolicitud = new DateTime(2025, 12, 31) }
        };

        // Act (Lógica definida en readFilterByEstadoYFecha)
        Func<Reserva, bool> filtro = entity => entity.Estado == EstadoReserva.Pendiente && 
                                              entity.FechaSolicitud >= desde && 
                                              entity.FechaSolicitud < hasta;

        var resultado = reservas.Where(filtro).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);
    }
}