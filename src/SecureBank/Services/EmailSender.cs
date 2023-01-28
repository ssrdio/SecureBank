
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using NLog;
using SecureBank.Interfaces;
using SecureBank.Models;

namespace SecureBank.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly AppSettings _appSettings;

        public EmailSender(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if(_appSettings.IgnoreEmails)
            {
                return Task.CompletedTask;
            }

            try
            {
                SmtpClient client = new SmtpClient(_appSettings.SmtpCredentials.Ip, _appSettings.SmtpCredentials.Port);
                if (!String.IsNullOrEmpty(_appSettings.SmtpCredentials.Username) && _appSettings.SmtpCredentials.Username != "null")
                {
                    /*  
                     *    This is used for testing with outlook/gmail/yahoomail,...
                     */
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_appSettings.SmtpCredentials.Username, _appSettings.SmtpCredentials.Password);
                }

                _logger.Info($"Sending mail");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("securebank@ssrd.io");
                mailMessage.To.Add(email);
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = subject;
                client.Send(mailMessage);
            }
            catch(Exception ex)
            {
                _logger.Error($"Failed to send mail. {ex}");
            }

            return Task.CompletedTask;    
        }
    }
}
