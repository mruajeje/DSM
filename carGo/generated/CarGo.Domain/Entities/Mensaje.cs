using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CarGo.Domain.Entities;

public partial class Mensaje
{
    public virtual int Id { get; set; }
    public virtual string Contenido { get; set; } = string.Empty;
    public virtual DateTime FechaHora { get; set; }
    public virtual Viaje Viaje { get; set; } = null!;
    public virtual Usuario Remitente { get; set; } = null!;

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Mensaje other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
