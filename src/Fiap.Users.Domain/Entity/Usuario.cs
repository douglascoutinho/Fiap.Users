namespace Fiap.Users.Domain.Entity
{
  public class Usuario : EntityBase
  {
    protected Usuario() { }

    public Usuario(string? nome, string? email, string? senha, string? perfil)
    {
      this.Nome = nome;
      this.Email = email;
      this.Senha = senha;
      this.Perfil = perfil;
      this.Ativo = true;
      this.DataCadastro = DateTime.Now;
    }

    public Usuario(int id, string? nome, string? email, string? senha, string? perfil, bool ativo, DateTime dataCadastro)
    {
      this.Id = id;
      this.Nome = nome;
      this.Email = email;
      this.Senha = senha;
      this.Perfil = perfil;
      this.Ativo = ativo;
      this.DataCadastro = dataCadastro;
    }

    public string? Nome { get; private set; }
    public string? Email { get; private set; }
    public string? Senha { get; private set; }
    public string? Perfil { get; private set; }

    internal void Atualizar(string? nome, string? email, string? senha, bool ativo)
    {
      this.Nome = nome;
      this.Email = email;
      this.Senha = senha;
      this.Ativo = ativo;
    }
  }
}
