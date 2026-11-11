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
    public async Task<string> LoginEmailAsync(string p_email, string p_password)
    {
        /*PROTECTED REGION ID(CarGoGen.Application.Services_Usuario_loginEmail) ENABLED START*/
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
