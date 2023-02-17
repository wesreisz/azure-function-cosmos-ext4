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


        [JsonProperty("_rid")]
        public string _rid { get; set; }


        [JsonProperty("_self")]
        public string _self { get; set; }


        [JsonProperty("_etag")]
        public string _etag { get; set; }


        [JsonProperty("_attachments")]
        public string _attachments { get; set; }


        [JsonProperty("_ts")]
        public string _ts { get; set; }
    }
}