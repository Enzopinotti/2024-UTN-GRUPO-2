using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;  // Para especificar la seguridad TLS
using System;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        public async Task SendEmailAsync(Message message)
        {
            var mailMessage= CreateEmailMessage(message);
            await SendAsync(mailMessage);
        }
        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            // Cambiar el cuerpo del email a formato HTML con enlace clickeable
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(
                    "<h2>Confirma tu cuenta</h2><p>Por favor confirma tu cuenta haciendo clic en el siguiente enlace: <a href='{0}'>Confirmar email</a></p>",
                    message.Content)
            };

            return emailMessage;
        }


        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    // Conectar usando STARTTLS
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);

                    // Eliminar "XOAUTH2" de los mecanismos de autenticación si no es necesario
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Autenticación utilizando usuario y contraseña (que debe ser la contraseña de aplicación)
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    // Enviar el correo
                    client.Send(mailMessage);
                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine($"SMTP Command Error: {ex.Message}, StatusCode: {ex.StatusCode}");
                    throw new Exception($"SMTP Command Error: {ex.Message}", ex);
                }
                catch (SmtpProtocolException ex)
                {
                    Console.WriteLine($"SMTP Protocol Error: {ex.Message}");
                    throw new Exception($"SMTP Protocol Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);  // Esto puede ser útil para obtener más detalles
                    throw new Exception($"Error general: {ex.Message}", ex);
                }
                finally
                {
                    client.Disconnect(true);  // Asegúrate de desconectar después de enviar el correo
                    client.Dispose();  // Liberar recursos
                }
            }
        }
private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    // Conectar usando STARTTLS
                  await  client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);

                    // Eliminar "XOAUTH2" de los mecanismos de autenticación si no es necesario
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Autenticación utilizando usuario y contraseña (que debe ser la contraseña de aplicación)
                   await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    // Enviar el correo
                    await client.SendAsync(mailMessage);
                }
                catch (SmtpCommandException ex)
                {
                    Console.WriteLine($"SMTP Command Error: {ex.Message}, StatusCode: {ex.StatusCode}");
                    throw new Exception($"SMTP Command Error: {ex.Message}", ex);
                }
                catch (SmtpProtocolException ex)
                {
                    Console.WriteLine($"SMTP Protocol Error: {ex.Message}");
                    throw new Exception($"SMTP Protocol Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);  // Esto puede ser útil para obtener más detalles
                    throw new Exception($"Error general: {ex.Message}", ex);
                }
                finally
                {
                  await client.DisconnectAsync(true);  // Asegúrate de desconectar después de enviar el correo
                    client.Dispose();  // Liberar recursos
                }
            }
        }
    }
}
