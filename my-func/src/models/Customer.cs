using Newtonsoft.Json;

namespace CosmosDBSamplesV2
{
    public class Customer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("customerEmail")]
        public string CustomerEmail { get; set; }

        [JsonProperty("customerPhone")]
        public string CustomerPhone { get; set; }
    }
}