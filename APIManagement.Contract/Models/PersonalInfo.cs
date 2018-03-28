using Newtonsoft.Json;

namespace APIManagement.Contract
{
    /// <summary>
    /// 
    /// </summary>
    public class PersonalInfo : APIMBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("DocumentType")]
        public string DocumentType => nameof(PersonalInfo);
    }
}
