namespace HotelReservationSystem.Services;

// Demo implementation. Replace this with SMTP/SendGrid in production.
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendReceiptAsync(string email, string subject, string body)
    {
        _logger.LogInformation("Sending receipt to {Email}: {Subject} - {Body}", email, subject, body);
        return Task.CompletedTask;
    }
}
