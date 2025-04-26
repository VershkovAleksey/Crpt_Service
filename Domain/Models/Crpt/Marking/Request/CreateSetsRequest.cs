using System.Text.Json.Serialization;

namespace Domain.Models.Crpt.Marking.Request;

public class CreateSetsRequest
{
    /// <summary>
    /// ИНН участника оборота товаров
    /// </summary>
    public required string ParticipantId { get; set; }

    /// <summary>
    /// Список формируемых наборов
    /// </summary>
    public required List<AggregationUnit> AggregationUnits { get; set; }
}

public class AggregationUnit
{
    /// <summary>
    /// КИН состоит от 18 до 74 символов включительно: цифры, буквы латинского алфавита, спецсимволы
    /// </summary>
    public required string UnitSerialNumber { get; set; }

    /// <summary>
    /// Список КИ / КИК, входящих в набор
    /// </summary>
    public required string[] Sntins { get; set; }
}