using Fiap.Users.Api.Models.Auth;
using Fiap.Users.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Users.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class AuthController(IConfiguration configuration, AuthService authService) : ControllerBase
  {
    readonly IConfiguration _configuration = configuration;
    readonly AuthService _authService = authService;

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="dto">Corpo da requisição</param>
    /// <returns></returns>
    /// <response code="200"></response>
    [HttpPost("")]
    [ProducesResponseType(typeof(string), 200)]
    [AllowAnonymous]
    public IActionResult Login([FromBody] PostAuthRequestDto dto)
    {
      var token = _authService.Autenticar
      (
        login: dto.Login,
        senha: dto.Senha,
        Key: _configuration["Jwt:Key"],
        issuer: _configuration["Jwt:Issuer"]
      );

      return Ok(new { token });
    }
  }
}