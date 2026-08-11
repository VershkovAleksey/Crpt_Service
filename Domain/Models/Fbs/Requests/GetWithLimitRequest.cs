using Newtonsoft.Json;

namespace Domain.Models.Fbs.Requests;

/// <summary>
/// Запрос на получение списка поставок
/// </summary>
public class GetWithLimitRequest
{
    /// <summary>
    /// Параметр пагинации. Устанавливает предельное количество возвращаемых данных.
    /// </summary>
    [JsonProperty("limit")]
    public int Limit { get; set; } = 100;

    /// <summary>
    /// Параметр пагинации. Устанавливает значение, с которого надо получить следующий пакет данных.
    /// Для получения полного списка данных должен быть равен 0 в первом запросе.
    /// Для следующих запросов необходимо брать значения из одноимённого поля в ответе.
    /// </summary>
    [JsonProperty("next")]
    public long Next { get; set; } = 0;
}