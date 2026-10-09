using Fiap.Users.Domain.Entity;
using Fiap.Users.Domain.Repositories;
using Fiap.Users.Infra.Data.EntityFramework;

namespace Fiap.Users.Infra.Repositories
{
  public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
  {
    readonly Contexto? _contexto;

    public UsuarioRepository(Contexto? contexto) : base(contexto)
    {
      _contexto = contexto;
    }
  }
}
