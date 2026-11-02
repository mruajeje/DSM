using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CarGoGen.Domain.ValueObjects;
using CarGoGen.Domain.Enums;

namespace CarGoGen.Domain.Entities;

public partial class Viaje
{
    public virtual int Id { get; set; }
    public virtual Ubicacion Origen { get; set; } = null!;
    public virtual Ubicacion Destino { get; set; } = null!;
    public virtual DateTime FechaHoraSalida { get; set; }
    public virtual int NumPlazasDisponibles { get; set; }
    public virtual Dinero Precio { get; set; } = null!;
    public virtual bool PermiteMascotas { get; set; }
    public virtual bool PermiteEquipaje { get; set; }
    public virtual string? Observaciones { get; set; }
    public virtual EstadoViaje Estado { get; set; }
    public virtual Usuario Conductor { get; set; } = null!;
    public virtual IList<Usuario> Pasajero { get; set; } = new List<Usuario>();
    public virtual IList<Reserva> Reservas { get; set; } = new List<Reserva>();
    public virtual IList<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    public virtual IList<Resena> Resenas { get; set; } = new List<Resena>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Viaje other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
