using RabbitMQ.Client;

namespace Fiap.Users.Api.Services.Extensions
{
  public static class Extension
  {
    public static ConnectionFactory CarregarConnectionFactory(this IConfiguration config)
    {
      var section = config.GetSection("RabbitMQ");

      return new ConnectionFactory
      {
        HostName = ValorOuPadrao(section["Host"], "localhost"),
        Port = int.TryParse(section["Port"], out var port) ? port : 5672,
        UserName = ValorOuPadrao(section["User"], "admin"),
        Password = ValorOuPadrao(section["Password"], "admin1234"),
        VirtualHost = ValorOuPadrao(section["VirtualHost"], "/")
      };
    }

    static string ValorOuPadrao(string? valor, string padrao) => string.IsNullOrWhiteSpace(valor) ? padrao : valor;
  }
}

