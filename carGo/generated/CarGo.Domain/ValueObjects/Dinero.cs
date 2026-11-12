using System.Collections.Generic;
using CarGo.Domain.Enums;

namespace CarGo.Domain.ValueObjects;

public partial class Dinero
{
    public virtual EnumMoneda Moneda { get; set; }
    public virtual double Cantidad { get; set; }

    public override bool Equals(object? obj) => obj is Dinero other &&
        EqualityComparer<EnumMoneda>.Default.Equals(Moneda, other.Moneda) &&
        EqualityComparer<double>.Default.Equals(Cantidad, other.Cantidad);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Moneda);
        hash.Add(Cantidad);
        return hash.ToHashCode();
    }
}
