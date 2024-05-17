using MimeKit;
using System.Linq;

namespace Talabat.APIs.Models
{
    public class Message
    {
        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x =>
            {
                return new MailboxAddress("Email", x);
            }));
            Subject = subject;
            Content = content;
        }

        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        
    }
}
