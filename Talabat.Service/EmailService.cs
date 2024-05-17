using MimeKit;
using Talabat.APIs.Models;
using Talabat.Core.Services.Interfaces;
namespace Talabat.Service;
using MailKit.Net.Smtp;

public class EmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;

    public EmailService(EmailConfig emailConfig)
    {
        _emailConfig = emailConfig;
    }
    public void SendEmail(Message message)
    {
        var EmailMessage=CreateEmailMessage(message);
    }

    private object CreateEmailMessage(Message message)
    {
        var EmailMessage = new MimeMessage();
        EmailMessage.From.Add(new MailboxAddress("Email", _emailConfig.From));
        EmailMessage.To.AddRange(message.To);
        EmailMessage.Subject = message.Subject;
        EmailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
        return EmailMessage;
    }
    private void Send(MimeMessage message)
    {
        using var Client = new SmtpClient();
        try
        {
            Client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
            Client.AuthenticationMechanisms.Remove("XOAUTH2");
            Client.Authenticate(_emailConfig.Username, _emailConfig.Password);
            Client.Send(message);
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            Client.Disconnect(true);
            Client.Dispose();
        }
    }
}
