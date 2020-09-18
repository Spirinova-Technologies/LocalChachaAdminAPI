using Newtonsoft.Json;

namespace LocalChachaAdminApi.Core.Models
{
    public class CreateMessageRequest
    {
        [JsonProperty("chat_dialog_id")]
        public string ChatDialogId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}