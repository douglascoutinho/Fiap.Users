namespace Fiap.Users.Api.Models.Usuario
{
  public class PutUsuarioRequestDto
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
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }
  }
}
