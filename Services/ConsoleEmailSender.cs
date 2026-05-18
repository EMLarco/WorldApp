using Microsoft.Extensions.Logging;

namespace WorldApp.Services;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation($"Enviando email a {to}: {subject}\n{body}");
        return Task.CompletedTask;
    }
}