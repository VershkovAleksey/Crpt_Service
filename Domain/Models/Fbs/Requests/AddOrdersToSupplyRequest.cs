using Newtonsoft.Json;

namespace Domain.Models.Fbs.Requests
{
    public sealed class AddOrdersToSupplyRequest
    {
        [JsonProperty("orders")]
        public List<long> Orders { get; set; }
    }
}
