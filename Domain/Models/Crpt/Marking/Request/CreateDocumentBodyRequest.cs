using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Models.Crpt.Marking.Request;

public class CreateDocumentBodyRequest
{
    /// <summary>
    /// Тип документа
    /// <value>Значения:
    /// MANUAL – формат * .json;
    /// XML – формат * .xml;
    /// CSV – формат * .csv</value>
    /// </summary>
    [JsonPropertyName("document_format")]
    public string DocumentFormat { get; set; } = "MANUAL";
    
    /// <summary>
    /// Тело формируемого документа, закодированное в base64
    /// </summary>
    [JsonPropertyName("product_document")]
    public required string ProductDocument { get; set; }
    
    /// <summary>
    /// Код типа документа
    /// </summary>
    public string Type { get; set; } = "SETS_AGGREGATION";
    
    /// <summary>
    /// Откреплённая подпись (УКЭП) в формате base64
    /// <remarks>Для типов документов «FIXATION», «UNIVERSAL_TRANSFER_DOCUMENT», «UNIVERSAL_TRANSFER_DOCUMENT_FIX» указывается откреплённая подпись титула продавца</remarks>
    /// </summary>
    public required string Signature { get; set; }
}