using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Domain.Entities;

public partial class Usuario
{
    public virtual int Id { get; set; }
    public virtual string Password { get; set; } = string.Empty;
    public virtual string NombreCompleto { get; set; } = string.Empty;
    public virtual string Email { get; set; } = string.Empty;
    public virtual string? Telefono { get; set; }
    public virtual string? Dni { get; set; }
    public virtual bool EsAdmin { get; set; }
    public virtual EstadoVerificacion EstadoVerificacion { get; set; }
    public virtual IList<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
    public virtual IList<Viaje> Viajes { get; set; } = new List<Viaje>();
    public virtual IList<Reserva> Reservas { get; set; } = new List<Reserva>();
    public virtual IList<Mensaje> MensajesEnviados { get; set; } = new List<Mensaje>();
    public virtual IList<Resena> ResenasEscritas { get; set; } = new List<Resena>();
    public virtual IList<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Usuario other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
