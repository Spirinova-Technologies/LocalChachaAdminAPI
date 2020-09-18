using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LocalChachaAdminApi.Core.Models
{
    public class CreateDialogueResponse
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("last_message")]
        public object LastMessage { get; set; }

        [JsonProperty("last_message_date_sent")]
        public object LastMessageDateSent { get; set; }

        [JsonProperty("last_message_id")]
        public object LastMessageId { get; set; }

        [JsonProperty("last_message_user_id")]
        public object LastMessageUserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("occupants_ids")]
        public List<long> OccupantsIds { get; set; }

        [JsonProperty("photo")]
        public object Photo { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("xmpp_room_jid")]
        public object XmppRoomJid { get; set; }

        [JsonProperty("unread_messages_count")]
        public long UnreadMessagesCount { get; set; }
    }
}