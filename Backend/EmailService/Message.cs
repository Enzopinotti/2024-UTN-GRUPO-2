using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class Message
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }

    public Message(IEnumerable<string> to, string subject, string content, object value)
    {
        To = new List<MailboxAddress>();
        // Agregar cada dirección de correo al objeto MailboxAddress
        foreach (var email in to)
        {
            To.Add(new MailboxAddress(string.Empty, email)); // Nombre vacío y correo
        }
        Subject = subject;
        Content = content;
    }
}
