using System.Collections.Generic;

namespace CarGo.Domain.ValueObjects;

public partial class Ubicacion
{
    public virtual string Ciudad { get; set; } = string.Empty;
    public virtual string Direccion { get; set; } = string.Empty;
    public virtual double Latitud { get; set; }
    public virtual double Longitud { get; set; }

    public override bool Equals(object? obj) => obj is Ubicacion other &&
        EqualityComparer<string>.Default.Equals(Ciudad, other.Ciudad) &&
        EqualityComparer<string>.Default.Equals(Direccion, other.Direccion) &&
        EqualityComparer<double>.Default.Equals(Latitud, other.Latitud) &&
        EqualityComparer<double>.Default.Equals(Longitud, other.Longitud);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Ciudad);
        hash.Add(Direccion);
        hash.Add(Latitud);
        hash.Add(Longitud);
        return hash.ToHashCode();
    }
}
