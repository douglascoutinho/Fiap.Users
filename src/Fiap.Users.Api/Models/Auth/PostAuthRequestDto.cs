namespace Fiap.Users.Api.Models.Auth
{
  public class PostAuthRequestDto
  {
    /// <summary>
    /// Login
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    ///   Senha
    /// </summary>
    public string? Senha { get; set; }
  }
}
