using Fiap.Users.Domain.Entity;

namespace Fiap.Users.Domain.Services.Extensions
{
  public static class Extension
  {
    public static bool ValidarEmail(this Usuario usuario)
    {
      var email = usuario.Email;
      if (string.IsNullOrEmpty(email))
        return false;

      return email.Contains("@") && email.Contains(".");
    }

    public static bool ValidarSenha(this Usuario usuario)
    {
      var senha = usuario.Senha;

      if (string.IsNullOrWhiteSpace(senha)) return false;
      if (senha.Length < 8) return false;

      bool temLetra = senha.Any(char.IsLetter);
      bool temNumero = senha.Any(char.IsDigit);
      bool temEspecial = senha.Any(ch => !char.IsLetterOrDigit(ch));

      return temLetra && temNumero && temEspecial;
    }
  }
} 
