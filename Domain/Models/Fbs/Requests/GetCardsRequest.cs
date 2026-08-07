using Newtonsoft.Json;

namespace Domain.Models.Fbs.Requests
{
    public class GetCardsRequest
    {
        [JsonProperty("settings")]
        public Settings Settings { get; set; } = new();
    }

    public class Sort
    {
        [JsonProperty("ascending")]
        public bool Ascending { get; set; } = false;
    }

    public class Cursor
    {
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("nmID")]
        public int NmID { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; } = 100;
    }

    public class Filter
    {
        [JsonProperty("textSearch")]
        public string TextSearch { get; set; }

        [JsonProperty("allowedCategoriesOnly")]
        public bool AllowedCategoriesOnly { get; set; }

        [JsonProperty("tagIDs")]
        public List<int> TagIDs { get; set; }

        [JsonProperty("objectIDs")]
        public List<int> ObjectIDs { get; set; }

        [JsonProperty("brands")]
        public List<string> Brands { get; set; }

        [JsonProperty("imtID")]
        public int ImtID { get; set; }

        [JsonProperty("withPhoto")]
        public int WithPhoto { get; set; } = -1;
    }

    public class Settings
    {
        [JsonProperty("sort")]
        public Sort Sort { get; set; }

        [JsonProperty("cursor")]
        public Cursor Cursor { get; set; } = new();

        [JsonProperty("filter")]
        public Filter Filter { get; set; }
    }
}
