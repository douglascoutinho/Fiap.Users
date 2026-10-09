using Fiap.Users.Domain.Entity;
using Fiap.Users.Domain.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Fiap.Users.Api.Services
{
  public class GenerateToken : IGenerateToken
  {
    public string? Autenticar(Usuario usuario, string jwtKey, string jwtIssuer)
    {
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.Email ?? string.Empty),
        new Claim("identificador-usuario", usuario.Id.ToString() ?? string.Empty),
        new Claim(ClaimTypes.Role, usuario.Perfil ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken
      (
        issuer: jwtIssuer,
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
} 