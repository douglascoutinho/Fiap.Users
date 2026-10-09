namespace Fiap.Users.Domain.Services.Interface
{
    public interface IRabbitMQService
    {
        Task SendMessageAsync(string message);
    }
}
