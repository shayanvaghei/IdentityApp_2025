using API.DTOs;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(EmailSendDto model)
        {
            try
            {
                var username = _config["Email:Username"];
                var password = _config["Email:Password"];

                using var client = new SmtpClient("smtp.zohocloud.ca", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(username, password)
                };

                var message = new MailMessage(from: username, to: model.To, subject: model.Subject, body: model.Body);
                message.IsBodyHtml = true;
                await client.SendMailAsync(message);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
