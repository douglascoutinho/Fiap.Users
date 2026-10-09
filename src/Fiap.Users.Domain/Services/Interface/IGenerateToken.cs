using Fiap.Users.Domain.Entity;

namespace Fiap.Users.Domain.Services.Interface
{
  public interface IGenerateToken
  {
    string Autenticar(Usuario usuario, string? Key, string? issuer);
  }
}