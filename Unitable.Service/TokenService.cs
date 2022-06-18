using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Unitable.Dto.Request;

namespace Unitable.Service
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, DtoLoginUsuario usuario);
    }

    public class TokenService : ITokenService
    {
        private TimeSpan ExpiryDuration = new TimeSpan(2, 0, 0);

        public string BuildToken(string key, string issuer, DtoLoginUsuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims, expires: DateTime.Now.Add(ExpiryDuration),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
