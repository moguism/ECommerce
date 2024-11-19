using System.Net;
using System.Net.Mail;

namespace Server.Services
{
    public class EmailService
    {
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;
        private const string EMAIL_FROM = "farminhouse447@gmail.com";
        private const string PASSWORD_EMAIL_FROM = "weiqxfzteoxaysbu";
        public  async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            using SmtpClient client = new SmtpClient(SMTP_HOST, SMTP_PORT)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(EMAIL_FROM, PASSWORD_EMAIL_FROM)
            };

            MailMessage mail = new MailMessage(EMAIL_FROM, to, subject, body)
            {
                IsBodyHtml = isHtml,
            };

            await client.SendMailAsync(mail);
        }
    }
}
