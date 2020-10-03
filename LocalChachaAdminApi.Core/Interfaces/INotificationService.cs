using LocalChachaAdminApi.Core.Models;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendEmail(Email email);
        Task<bool> SendSms();
    }
}