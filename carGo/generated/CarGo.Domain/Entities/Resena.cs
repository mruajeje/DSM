using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CarGo.Domain.Enums;

namespace CarGo.Domain.Entities;

public partial class Resena
{
    public virtual int Id { get; set; }
    public virtual int Puntuacion { get; set; }
    public virtual string? Comentario { get; set; }
    public virtual DateTime FechaCreacion { get; set; }
    public virtual EstadoResena Estado { get; set; }
    public virtual string? MotivoModeracion { get; set; }
    public virtual Viaje Viaje { get; set; } = null!;
    public virtual Usuario Autor { get; set; } = null!;

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Resena other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
