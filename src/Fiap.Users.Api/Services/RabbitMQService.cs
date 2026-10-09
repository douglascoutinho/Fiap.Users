using Fiap.Users.Domain.Services.Interface;
using RabbitMQ.Client;
using System.Text;

namespace Fiap.Users.Api.Services
{
  public class RabbitMQService : IRabbitMQService
  {
    private readonly IConfiguration _configuration;
    private readonly string? _hostName;
    private readonly string? _userName;
    private readonly string? _password;
    private readonly string? _queueName;

    public RabbitMQService(IConfiguration configuration)
    {
      _configuration = configuration;
      _hostName = _configuration["RabbitMQ:HostName"];
      _userName = _configuration["RabbitMQ:UserName"];
      _password = _configuration["RabbitMQ:Password"];
      _queueName = _configuration["RabbitMQ:QueueName"];
    }

    public async Task SendMessageAsync(string message)
    {
      var factory = new ConnectionFactory()
      {
        HostName = _hostName ?? string.Empty,
        UserName = _userName ?? string.Empty,
        Password = _password ?? string.Empty
      };

      using var connection = await factory.CreateConnectionAsync();
      using var channel = await connection.CreateChannelAsync();

      // Declara a fila (criar se não existir)
      await channel.QueueDeclareAsync
      (
          queue: _queueName ?? string.Empty,
          durable: true,
          exclusive: false,
          autoDelete: false,
          arguments: null
      );

      // Prepara a mensagem
      var messageBody = Encoding.UTF8.GetBytes(message);

      // Envia a mensagem
      await channel.BasicPublishAsync
      (
        exchange: "",
        routingKey: _queueName ?? string.Empty,
        body: messageBody
      );
    }
  }
}
