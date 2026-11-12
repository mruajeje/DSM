using CarGo.Application.Abstractions;
using CarGo.Application.Exceptions;
using CarGo.Domain.Entities;
using CarGo.Domain.Enums;

        /*PROTECTED REGION ID(usingCarGo.Application.Services_Usuario_loginEmail) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace CarGo.Application.Services;

public sealed partial class UsuarioService
{
    public async Task<string> LoginEmailAsync(string p_email, string p_password)
    {
        /*PROTECTED REGION ID(CarGo.Application.Services_Usuario_loginEmail) ENABLED START*/
        try
        {
            var usuarios = await _repository.ReadFilterByEmailAsync(p_email);
            Usuario? entity = usuarios.FirstOrDefault();
            if (entity is null)
            {
                return string.Empty;
            }

            if (!_passwordHasher.VerifyPassword(entity.Password, p_password))
            {
                return string.Empty;
            }

            var tokenSubject = Convert.ToString(entity.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var tokenClaims = new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["id"] = Convert.ToString(entity.Id, System.Globalization.CultureInfo.InvariantCulture),
            };
            return _unitOfWork.TokenGenerator.CreateToken(tokenSubject, tokenClaims);
        }
        catch (Exception)
        {
            return string.Empty;
        }
        /*PROTECTED REGION END*/
    }
}
