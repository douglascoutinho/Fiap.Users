using Fiap.Users.Api.Models.Usuario;
using Fiap.Users.Domain.Entity;
using Fiap.Users.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Users.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize(Roles = "Administrador")]
  public class UsuarioController : ControllerBase
  {
    readonly UsuarioService _usuarioService;

    public UsuarioController(UsuarioService usuarioService)
    {
      _usuarioService = usuarioService;
    }

    /// <summary>
    /// Retorna lista de Usuários
    /// </summary>
    /// <returns>Lista de Usuários</returns>
    /// <remarks></remarks>
    /// <response code="200">Lista de Todos as Usuários Encontrados</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(GetUsuariosResponseDto[]), 200)]
    [AllowAnonymous]
    public IActionResult Get(
      [FromQuery] bool? ativado,
      [FromQuery] int offSet = 0,
      [FromQuery] int limit = 50)
    {
      IEnumerable<Usuario> dados = _usuarioService.Buscar(ativado, offSet, limit);

      var dto = dados.Select(x => new GetUsuariosResponseDto
      {
        Id = x.Id,
        Nome = x.Nome,
        Email = x.Email,
        Ativo = x.Ativo,
        DataCadastro = x.DataCadastro
      }).ToArray();

      return Ok(dto);
    }

    /// <summary>
    /// Retorna Um Usuário
    /// </summary>
    /// <returns>Usuários</returns>
    /// <remarks></remarks>
    /// <response code="200">Usuário Encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetUsuarioResponseDto), 200)]
    public IActionResult GetById([FromRoute] int id)
    {
      var usuario = _usuarioService.BuscarPorId(id);

      var dto = new GetUsuarioResponseDto
      {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Ativo = usuario.Ativo,
        DataCadastro = usuario.DataCadastro
      };

      return Ok(dto);
    }


    /// <summary>
    /// Inclui um Usuário
    /// </summary>
    /// <param name="dto">Corpo da requisição</param>
    /// <returns></returns>
    /// <response code="200"></response>
    [HttpPost("")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Post([FromBody] PostUsuarioRequestDto dto)
    {
      var usuario = new Usuario(dto.Nome, dto.Email, dto.Senha, dto.Perfil.ToString());  

      _usuarioService.Incluir(usuario);

      return Ok("Usuário Cadastrado Com Sucesso.");
    }

    /// <summary>
    /// Altera um Usuário
    /// </summary>
    /// <param name="id">Identificador do Usuário</param>
    /// <param name="put">Corpo com dados a serem modificados</param>
    /// <returns></returns>
    /// <response code="200"></response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Put([FromRoute] int id, PutUsuarioRequestDto put)
    {
      var usuarioExistente = _usuarioService.BuscarPorId(id);

      _usuarioService.Alterar(
        usuarioExistente,
        put.Nome,
        put.Email,
        put.Senha,
        put.Ativo
      );

      return Ok("Usuário Atualizado Com Sucesso.");
    }
  }
}