namespace Fiap.Users.Domain.Services.Interface
{
  public interface IRabbitMQService
  {
    Task SendMessageAsync<T>(string targetQueue, T messageObject);
  }
}
