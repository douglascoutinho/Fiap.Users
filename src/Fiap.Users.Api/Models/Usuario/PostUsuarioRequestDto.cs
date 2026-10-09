using Fiap.Users.Domain.Entity.Enum;

namespace Fiap.Users.Api.Models.Usuario
{
  public class PostUsuarioRequestDto
  {
    /// <summary>
    /// Nome  
    /// </summary>
    public string? Nome { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Senha
    /// </summary>
    public string? Senha { get; set; }

    /// <summary>
    /// Perfil
    /// </summary>
    public PerfilEnum Perfil { get; set; }
  }
}
