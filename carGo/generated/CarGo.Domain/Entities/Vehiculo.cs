using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CarGo.Domain.Entities;

public partial class Vehiculo
{
    public virtual int Id { get; set; }
    public virtual string MarcaModelo { get; set; } = string.Empty;
    public virtual string Matricula { get; set; } = string.Empty;
    public virtual string? InformacionAdicional { get; set; }
    public virtual Usuario Propietario { get; set; } = null!;

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Vehiculo other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
