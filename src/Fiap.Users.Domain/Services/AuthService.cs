using Fiap.Users.Domain.Repositories;
using Fiap.Users.Domain.Services.Interface;
using System.Security.Authentication;

namespace Fiap.Users.Domain.Services
{
  public class AuthService
  {
    IUsuarioRepository _usuarioRepository;
    IGenerateToken _generateToken;

    public AuthService(IUsuarioRepository usuarioRepository, IGenerateToken generateToken)
    {
      _usuarioRepository = usuarioRepository;
      _generateToken = generateToken;
    }

    public string Autenticar(string? login, string? senha, string? Key, string? issuer)
    {
      var usuarioExistente =  _usuarioRepository.Get(x => x.Email == login && x.Senha == senha).FirstOrDefault();

      if (usuarioExistente == null) throw new InvalidCredentialException("Usuário e/ou senha invalido(s).");

      var token = _generateToken.Autenticar(usuarioExistente, Key, issuer);

      return token.ToString();
    }
  }
}
