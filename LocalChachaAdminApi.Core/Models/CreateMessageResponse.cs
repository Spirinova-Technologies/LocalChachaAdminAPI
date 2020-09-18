using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LocalChachaAdminApi.Core.Models
{
    public class CreateMessageResponse
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("attachments")]
        public List<object> Attachments { get; set; }

        [JsonProperty("chat_dialog_id")]
        public string ChatDialogId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("date_sent")]
        public long DateSent { get; set; }

        [JsonProperty("delivered_ids")]
        public List<long> DeliveredIds { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("read_ids")]
        public List<long> ReadIds { get; set; }

        [JsonProperty("recipient_id")]
        public long RecipientId { get; set; }

        [JsonProperty("sender_id")]
        public long SenderId { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("read")]
        public long Read { get; set; }
    }
}