using System;
using System.Collections.Generic;
using System.Linq;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;
using Xunit;

namespace CarGoGen.Tests.Manual;

public class ViajeFilterTests
{
    [Fact]
    public void ReadFilterViajesDisponibles_FiltraViajesPublicadosConPlazasYFechaValida()
    {
        // Arrange
        var fechaMinima = DateTime.Now;
        var viajes = new List<Viaje>
        {
            new Viaje { Id = 1, Estado = EstadoViaje.Publicado, NumPlazasDisponibles = 2, FechaHoraSalida = fechaMinima.AddHours(5) },
            new Viaje { Id = 2, Estado = EstadoViaje.Publicado, NumPlazasDisponibles = 0, FechaHoraSalida = fechaMinima.AddHours(5) },
            new Viaje { Id = 3, Estado = EstadoViaje.EnCurso, NumPlazasDisponibles = 3, FechaHoraSalida = fechaMinima.AddHours(5) },
            new Viaje { Id = 4, Estado = EstadoViaje.Publicado, NumPlazasDisponibles = 1, FechaHoraSalida = fechaMinima.AddDays(-1) }
        };

        // Act (Lógica definida en readFilterViajesDisponibles)
        Func<Viaje, bool> filtro = entity => entity.Estado == EstadoViaje.Publicado && 
                                             entity.NumPlazasDisponibles > 0 && 
                                             entity.FechaHoraSalida >= fechaMinima;

        var resultado = viajes.Where(filtro).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);
    }

    [Fact]
    public void ReadFilterByPreferenciasYEstado_FiltraPorMascotasEquipajeYEstadoValido()
    {
        // Arrange
        var viajes = new List<Viaje>
        {
            new Viaje { Id = 1, PermiteMascotas = true, PermiteEquipaje = true, Estado = EstadoViaje.Publicado },
            new Viaje { Id = 2, PermiteMascotas = false, PermiteEquipaje = true, Estado = EstadoViaje.Publicado },
            new Viaje { Id = 3, PermiteMascotas = true, PermiteEquipaje = true, Estado = EstadoViaje.Cancelado }
        };

        // Act (Lógica definida en readFilterByPreferenciasYEstado)
        bool p_permiteMascotas = true;
        bool p_permiteEquipaje = false;

        Func<Viaje, bool> filtro = entity => (!p_permiteMascotas || entity.PermiteMascotas) && 
                                             (!p_permiteEquipaje || entity.PermiteEquipaje) && 
                                             (entity.Estado == EstadoViaje.Publicado || entity.Estado == EstadoViaje.EnCurso);

        var resultado = viajes.Where(filtro).ToList();

        // Assert
        Assert.Single(resultado);
        Assert.Equal(1, resultado[0].Id);
    }
}