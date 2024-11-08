using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class Message
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }

    public Message(IEnumerable<string> to, string subject, string content)
    {
        To = new List<MailboxAddress>();
        foreach (var email in to)
        {
            To.Add(new MailboxAddress(string.Empty, email));
        }
        Subject = subject;
        Content = content;
    }

}
