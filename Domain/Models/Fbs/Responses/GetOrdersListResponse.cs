using Newtonsoft.Json;

namespace Domain.Models.Fbs.Responses;

public class GetOrdersListResponse
{
    /// <summary>
    /// Список заказов
    /// </summary>
    [JsonProperty("orders")] 
    public List<Order> Orders { get; set; } = new();
    
    /// <summary>
    /// Параметр пагинации. Содержит значение, которое необходимо указать в запросе для получения следующего пакета данных
    /// </summary>
    [JsonProperty("next")]
    public long Next { get; set; }
}