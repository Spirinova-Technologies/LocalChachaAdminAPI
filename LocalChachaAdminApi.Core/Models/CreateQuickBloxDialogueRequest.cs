using Newtonsoft.Json;

namespace LocalChachaAdminApi.Core.Models
{
    public class CreateQuickBloxDialogueRequest
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("occupants_ids")]
        public string OccupantsIds { get; set; }
    }
}