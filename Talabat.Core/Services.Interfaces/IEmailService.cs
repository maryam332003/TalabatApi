using Talabat.APIs.Models;

namespace Talabat.Core.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
