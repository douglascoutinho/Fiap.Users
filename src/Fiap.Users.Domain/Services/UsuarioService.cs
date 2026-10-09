using Fiap.Users.Domain.Entity;
using Fiap.Users.Domain.Repositories;
using Fiap.Users.Domain.Services.Extensions;
using Fiap.Users.Domain.Services.Interface;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Fiap.Users.Domain.Services
{
  public class UsuarioService
  {
    IUsuarioRepository _usuarioRepository;
    IRabbitMQService _rabbitMQ;

    public UsuarioService(IUsuarioRepository usuarioRepository, IRabbitMQService rabbitMQ)
    {
      _usuarioRepository = usuarioRepository;
      _rabbitMQ = rabbitMQ;
    }

    public ICollection<Usuario> Buscar(bool? ativo, int offSet, int limit)
    {
      var query = _usuarioRepository.Get();

      if (ativo != null)
        query = query.Where(a => a.Ativo == ativo);

      query = query.Skip(offSet).Take(limit);

      _rabbitMQ.SendMessageAsync("Usuario Consultado Com Sucesso....");

      return query.ToList();
    }

    public Usuario BuscarPorId(int id)
    {
      var usuario = _usuarioRepository.GetById(id);

      if (usuario == null)
        throw new ValidationException("Informado identificador que não existe.");

      return usuario;
    }

    public void Incluir(Usuario usuario)
    {
      var usuarioExistente = _usuarioRepository.GetById(usuario.Id);

      if (usuarioExistente != null)
        throw new ApplicationException("Informado identificador já existe.");

      if (!usuario.ValidarEmail())
        throw new ApplicationException("Email inválido.");

      if (!usuario.ValidarSenha())
        throw new ApplicationException("Senha deve ter no mínimo de 8 caracteres com números, letras e caracteres especiais");

      _usuarioRepository.Add(usuario);

      _usuarioRepository.Commit();
    }

    public void Alterar(Usuario usuarioExistente, string? nome, string? email, string? senha, bool ativo)
    {

      if (usuarioExistente == null)
        throw new ValidationException("Informado identificador que não existe.");

      usuarioExistente.Atualizar
      (
          nome,
          email,
          senha,
          ativo
      );

      _usuarioRepository.Update(usuarioExistente);

      _usuarioRepository.Commit();
    }
  }
}
