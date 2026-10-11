namespace Fiap.Users.Domain.Entity
{
  public class UsuarioEvent
  {
    public UsuarioEvent(string? nome, string? email)
    {
      Nome = nome;
      Email = email;
    }

    public string? Nome { get; private set; }
    public string? Email { get; private set; }
  }
}
