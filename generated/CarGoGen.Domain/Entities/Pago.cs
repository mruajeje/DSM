using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Domain.Entities;

public partial class Pago
{
    public virtual int Id { get; set; }
    public virtual Dinero Monto { get; set; } = null!;
    public virtual DateTime FechaPago { get; set; }
    public virtual EstadoPago Estado { get; set; }
    public virtual string? IdTransaccionPasarela { get; set; }
    public virtual Reserva Reserva { get; set; } = null!;

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Pago other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
