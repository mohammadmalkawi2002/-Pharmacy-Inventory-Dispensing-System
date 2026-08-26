using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PharmacyInventoryDispensingSystem.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Text;
using Microsoft.Extensions.Logging;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.Email
{
    public class EmailService(IOptions<SmtpSettings> options,
        ILogger<EmailService> logger) : IEmailService
    {
        private readonly SmtpSettings _settings=options.Value;
        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage,
            CancellationToken cancellationToken=default)

        {

            try
            {

                logger.LogInformation(
                    "Sending email to {Email} with subject {Subject}.",
                    email,
                    subject);

                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(
                    _settings.FromName,
                    _settings.FromEmail));

                message.To.Add(
                    MailboxAddress.Parse(email));

                message.Subject = subject;

                message.Body = new BodyBuilder
                {
                    HtmlBody = htmlMessage


                }.ToMessageBody();


                using var smtp = new SmtpClient();

                await smtp.ConnectAsync(_settings.Host,
                    _settings.Port,
                    _settings.UseSsl
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls,
                    cancellationToken);

                await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

                await smtp.SendAsync(message, cancellationToken);

                await smtp.DisconnectAsync(true, cancellationToken);

                logger.LogInformation(
                "Email sent successfully to {Email}.",
                email);

            }

            catch(Exception ex) 
            {
                logger.LogError(ex, "Failed to send email to {Email} .",email);

                throw;
            
            }



        }
    }
}
