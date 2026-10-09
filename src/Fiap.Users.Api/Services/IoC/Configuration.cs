using Fiap.Users.Domain.Services.Interface;
using Fiap.Users.Infra.Repositories;

namespace Fiap.Users.Api.Services.IoC
{
	/// <summary>
	/// Classe de configuração de IoC
	/// </summary>
	public static class Configuration
	{
		/// <summary>
		/// Registrar as dependencias dentro do motor de IoC
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
		{
      //Interfaces
      services.AddScoped<Domain.Services.Interface.IGenerateToken, GenerateToken>();
      services.AddScoped<Domain.Services.Interface.IRabbitMQService, RabbitMQService>();

      // Repositórios
      services.AddScoped<Domain.Repositories.IUsuarioRepository, UsuarioRepository>();

      // Domain Services			
      services.AddScoped<Domain.Services.UsuarioService>();
      services.AddScoped<Domain.Services.AuthService>();

      return services;
		}
	}
}
