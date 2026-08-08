using Newtonsoft.Json;

namespace Domain.Models.Fbs.Models
{

    public class CardList
    {
        [JsonProperty("cards")]
        public List<Cards> Cards { get; set; } = new();

        [JsonProperty("cursor")]
        public Cursor Cursor { get; set; } = new();
    }

    public class Cursor
    {
        [JsonProperty("nmID")]
        public int NmID { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }

    public class Characteristics
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public List<string> Value { get; set; }
    }

    public class Sizes
    {
        [JsonProperty("chrtID")]
        public int ChrtId { get; set; }

        [JsonProperty("techSize")]
        public string TechSize { get; set; }

        [JsonProperty("wbSize")]
        public string WbSize { get; set; }

        [JsonProperty("skus")]
        public List<string> Skus { get; set; }
    }

    public class Cards
    {
        [JsonProperty("nmID")]
        public int NmID { get; set; }

        [JsonProperty("imtID")]
        public long ImtID { get; set; }

        [JsonProperty("nmUUID")]
        public string NmUUID { get; set; }

        [JsonProperty("subjectID")]
        public int SubjectID { get; set; }

        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }

        [JsonProperty("vendorCode")]
        public string VendorCode { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("needKiz")]
        public bool NeedKiz { get; set; }

        [JsonProperty("kizMarked")]
        public bool KizMarked { get; set; }

        [JsonProperty("sizes")]
        public List<Sizes> Sizes { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }
}
