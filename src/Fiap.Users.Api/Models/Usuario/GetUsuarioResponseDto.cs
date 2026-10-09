namespace Fiap.Users.Api.Models.Usuario
{
  public class GetUsuarioResponseDto
  {
    /// <summary>
    /// Identificador do usuário
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome
    /// </summary>
    public string? Nome { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data Cadastro
    /// </summary>
    public DateTime DataCadastro { get; set; }
  }
}
