using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LocalChachaAdminApi.Core.Models
{
    public partial class QuickBloxDialogueResponse
    {
        [JsonProperty("total_entries")]
        public long TotalEntries { get; set; }

        [JsonProperty("skip")]
        public long Skip { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("last_message")]
        public string LastMessage { get; set; }

        [JsonProperty("last_message_date_sent")]
        public long LastMessageDateSent { get; set; }

        [JsonProperty("last_message_id")]
        public string LastMessageId { get; set; }

        [JsonProperty("last_message_user_id")]
        public long LastMessageUserId { get; set; }

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