using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace antigal.server.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly int _smtpPort;

        public EmailSender(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"];
            _smtpUser = configuration["EmailSettings:SmtpUser "];
            _smtpPass = configuration["EmailSettings:SmtpPass"];
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Your Name", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                // Conectar al servidor SMTP
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                // Autenticarse
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                // Enviar el mensaje
                await client.SendAsync(emailMessage);
                // Desconectar
                await client.DisconnectAsync(true);
            }
        }
    }
}
