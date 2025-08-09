using API.DTOs;
using System.Threading.Tasks;

namespace API.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailSendDto model);
    }
}
