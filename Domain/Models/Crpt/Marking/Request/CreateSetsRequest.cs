using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Domain.Models.Crpt.Marking.Request;

public class CreateSetsRequest
{
    /// <summary>
    /// ИНН участника оборота товаров
    /// </summary>
    [JsonPropertyName("participantId")]
    [JsonProperty("participantId")]
    public required string ParticipantId { get; set; }

    /// <summary>
    /// Список формируемых наборов
    /// </summary>
    [JsonPropertyName("aggregationUnits")]
    [JsonProperty("aggregationUnits")]
    public required List<AggregationUnit> AggregationUnits { get; set; }
}

public class AggregationUnit
{
    /// <summary>
    /// КИН состоит от 18 до 74 символов включительно: цифры, буквы латинского алфавита, спецсимволы
    /// </summary>
    [JsonPropertyName("unitSerialNumber")]
    [JsonProperty("unitSerialNumber")]
    public required string UnitSerialNumber { get; set; }

    /// <summary>
    /// Список КИ / КИК, входящих в набор
    /// </summary>
    [JsonPropertyName("sntins")]
    [JsonProperty("sntins")]
    public required string[] Sntins { get; set; }
}