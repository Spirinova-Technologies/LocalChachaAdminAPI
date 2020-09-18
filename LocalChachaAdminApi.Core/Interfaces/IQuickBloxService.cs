using LocalChachaAdminApi.Core.Models;
using System.Threading.Tasks;

namespace LocalChachaAdminApi.Core.Interfaces
{
    public interface IQuickBloxService
    {
        Task<string> GetQuickBloxToken(string username = "", string passowrd = "");

        Task<QuickbloxUser> CreateUser(QuickBloxUserRequest userRequest);

        Task DeleteUser(Merchant merchant);

        Task<QuickBloxDialogueResponse> GetDialogues(string token);

        Task DeleteDialogue(string dialogueId, string token);

        Task<Session> GetQuickBloxSession(string username = "", string passowrd = "");

        Task<CreateDialogueResponse> CreateDialogue(long userQuickBloxId, int merchantQuickBloxId, string token);

        Task<string> CreateMessage(CreateMessageRequest createMessageRequest, string token);
    }
}