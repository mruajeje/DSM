using System.Security.Claims;
using CarGo.Domain.Entities;

namespace CarGo.Web.Utils.Security;

public static partial class HariaWebAuthentication
{
    static partial void AddTrustedClaims(List<Claim> claims, string domainClass, object identifier, object? serverEntity)
    {
        if (serverEntity is not Usuario usuario) return;

        // Todo usuario autenticado es pasajero
        claims.Add(new Claim(ClaimTypes.Role, "Pasajero"));

        if (usuario.EsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        // Conductor: tiene al menos un vehículo
        if (usuario.Vehiculos.Count > 0)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Conductor"));
        }
    }
}