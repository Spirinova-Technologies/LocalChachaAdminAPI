using Newtonsoft.Json;

namespace LocalChachaAdminApi.Core.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("fullName")]
        public string fullName { get; set; }
    }
}