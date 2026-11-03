using CarGoGen.Application.Abstractions;
using CarGoGen.Application.Exceptions;
using CarGoGen.Domain.Entities;
using CarGoGen.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGoGen.Application.Services_Usuario_loginEmail) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGoGen.Application.Services;

public sealed partial class UsuarioService
{
    public async Task<string?> LoginEmailAsync(int p_oid, string p_password)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Usuario_loginEmail) ENABLED START*/
        try
        {
            Usuario? entity = await _repository.ReadAsync(p_oid);
            if (entity is null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(entity.Password, p_password))
            {
                return null;
            }

            var tokenSubject = Convert.ToString(entity.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var tokenClaims = new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["id"] = Convert.ToString(entity.Id, System.Globalization.CultureInfo.InvariantCulture),
            };
            return _tokenGenerator.CreateToken(tokenSubject, tokenClaims);
        }
        catch (Exception)
        {
            return null;
        }
        /*PROTECTED REGION END*/
    }
}
