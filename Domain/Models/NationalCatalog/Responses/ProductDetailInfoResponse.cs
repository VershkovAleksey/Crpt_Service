using Newtonsoft.Json;

namespace Domain.Models.NationalCatalog.Responses;

public class ProductDetailInfoResponse
{
    public int ApiVersion { get; set; }
    public required ProductDetailInfoResult[] Result { get; set; }
}

public class ProductDetailInfoResult
{
    [JsonProperty("good_id")] public string GoodId { get; set; }

    [JsonProperty("identified_by")] public required IEnumerable<IdentifiedBy> IdentifiedBy { get; set; }

    [JsonProperty("good_name")] public string? GoodName { get; set; }

    [JsonProperty("is_kit")] public bool IsKit { get; set; }
    
    public string? Gtin { get; set; }

    [JsonProperty("is_set")] public bool IsSet { get; set; }

    [JsonProperty("set_gtins")] public IEnumerable<SetGtins>? SetGtins { get; set; }

    [JsonProperty("good_url")] public string? GoodUrl { get; set; }

    [JsonProperty("good_img")] public string? GoodImg { get; set; }

    [JsonProperty("good_status")] public string GoodStatus { get; set; }

    [JsonProperty("good_detailed_status")] public IEnumerable<string> GoodDetailedStatus { get; set; }

    [JsonProperty("good_signed")] public required bool GoodSigned { get; set; }

    [JsonProperty("good_mark_flag")] public required bool GoodMarkFlag { get; set; }

    [JsonProperty("good_turn_flag")] public required bool GoodTurnFlag { get; set; }

    [JsonProperty("flags_updated_date")] public required string FlagUpdatedDate { get; set; }

    [JsonProperty("create_date")] public required string CreateDate { get; set; }

    [JsonProperty("update_date")] public required string UpdateDate { get; set; }

    [JsonProperty("producer_inn")] public required string ProducerInn { get; set; }

    [JsonProperty("producer_name")] public required string ProducerName { get; set; }

    public IEnumerable<Categories> Categories { get; set; }

    [JsonProperty("brand_id")] public required string BrandId { get; set; }

    [JsonProperty("brand_name")] public required string BrandName { get; set; }

    [JsonProperty("good_rating")] public string? GoodRating { get; set; }

    [JsonProperty("good_attrs")] public required IEnumerable<GoodAttrs> GoodAttrs { get; set; }
}

public class IdentifiedBy
{
    public required string Value { get; set; }
    public required string Type { get; set; }
    public required int Multiplier { get; set; }
    public required string Level { get; set; }
}

public class Categories
{
    [JsonProperty("cat_id")] public required int CatId { get; set; }

    [JsonProperty("cat_name")] public required string CatName { get; set; }
}

public class GoodAttrs
{
    [JsonProperty("attr_id")] public int AttrId { get; set; }

    [JsonProperty("attr_name")] public required string AttrName { get; set; }

    [JsonProperty("attr_value")] public required string AttrValue { get; set; }

    [JsonProperty("attr_value_id")] public required string AttrValueId { get; set; }

    [JsonProperty("attr_value_type")] public string? AttrValueType { get; set; }

    [JsonProperty("attr_group_id")] public int? AttrGroupId { get; set; }

    [JsonProperty("attr_group_name")] public required string AttrGroupName { get; set; }

    [JsonProperty("value_id")] public required string ValueId { get; set; }

    public string? Gtin { get; set; }

    public int? Multiplier { get; set; }

    public string? Level { get; set; }
}

public class SetGtins
{
    public string? Gtin { get; set; }
    public int? Quantity { get; set; }
}