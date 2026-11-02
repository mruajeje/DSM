using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Domain.Entities;

public partial class Reserva
{
    public virtual int Id { get; set; }
    public virtual int NumPlazasReservadas { get; set; }
    public virtual EstadoReserva Estado { get; set; }
    public virtual DateTime FechaSolicitud { get; set; }
    public virtual Viaje Viaje { get; set; } = null!;
    public virtual Usuario Pasajero { get; set; } = null!;
    public virtual Pago? Pago { get; set; }
    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Reserva other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
