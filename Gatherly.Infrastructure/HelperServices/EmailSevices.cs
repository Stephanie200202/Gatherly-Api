using Gatherly.Application.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Gatherly.Infrastructure.HelperServices;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpHost = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];
        var senderDisplayName = _configuration["EmailSettings:SenderName"] ?? "Gatherly Platform";

        var mailMessage = CreateMimeMessage(toEmail, subject, body, username!, senderDisplayName);

        using var client = new SmtpClient();
        try
        {
            // 🚀 THE FIX: Use Auto options so MailKit can dynamically scale the STARTTLS handshake
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.Auto);

            // Authenticate using your generated Google App Password
            await client.AuthenticateAsync(username, password);

            // Dispatch mail to the recipient
            await client.SendAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {Recipient}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to send an email to {Recipient}", toEmail);

            // 🔍 DEBUG TRAP: Prints the exact SMTP connection/credential error to your Output window
            System.Diagnostics.Debug.WriteLine("=== CRITICAL MAILKIT DELIVERY EXCEPTION ===");
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            System.Diagnostics.Debug.WriteLine("===========================================");

            throw new Exception($"Email delivery failed: {ex.Message}", ex);
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
        }
    }

    private MimeMessage CreateMimeMessage(string toEmail, string subject, string body, string senderEmail, string displayName)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(displayName, senderEmail));
        message.To.Add(new MailboxAddress(string.Empty, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }
}