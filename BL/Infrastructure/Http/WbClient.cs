using Domain.Models.Fbs;
using Domain.Models.Fbs.Models;
using Domain.Models.Fbs.Requests;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using WbManageBot.Models;

namespace BL.Infrastructure.Http;

/// <summary>
/// HTTP-клиент для взаимодействия с Wildberries openApi
/// </summary>
public class WbClient
{
    //private readonly string _token = "eyJhbGciOiJFUzI1NiIsImtpZCI6IjIwMjYwMzAydjEiLCJ0eXAiOiJKV1QifQ.eyJhY2MiOjIsImVudCI6MSwiZXhwIjoxODAxOTA0MTAyLCJpZCI6IjAxOWZkZTAyLTA1NjUtNzcxZS1hMzhmLWMxM2JhZmEzNTVlNiIsImlpZCI6MjcwODIxNTUsIm9pZCI6MTI4ODMzMiwicyI6MCwic2lkIjoiYjE4M2M0MGEtNzk1Yi00NDcyLThhNDktNGIzMmZjMmY1MTYzIiwidCI6dHJ1ZSwidWlkIjoyNzA4MjE1NX0.AVHs4Fs2DSrkE0Ul_5b_JX0-P7flKqwCHIo5YWVLmF9eQx__DNFi1Gc9dS-_r0xNfA5UDWGeydUGvpsJpQWjOQ";
    private readonly string _token = "eyJhbGciOiJFUzI1NiIsImtpZCI6IjIwMjYwMzAydjEiLCJ0eXAiOiJKV1QifQ.eyJhY2MiOjEsImVudCI6MSwiZXhwIjoxODAxOTAxMTk1LCJpZCI6IjAxOWZkZGQ1LWFhM2ItNzQzNy1iY2VhLTg0NTY1YWM3OTJjMyIsImlpZCI6MjcwODIxNTUsIm9pZCI6MTI4ODMzMiwicyI6MTgsInNpZCI6ImIxODNjNDBhLTc5NWItNDQ3Mi04YTQ5LTRiMzJmYzJmNTE2MyIsInQiOmZhbHNlLCJ1aWQiOjI3MDgyMTU1fQ.MXvzrN4_2ApQCsQznqse_mhEsOCCujcjDRi2gbr3n-S2IZFPewPPBr2Jzwdkd3zms1-P3aCec94Ir1_NsFRrlQ";
    private readonly ILogger<WbClient> _logger;
    private readonly string _contentApiUrl;
    private readonly string _marketplaceApiUrl;
    public WbClient(ILogger<WbClient> logger)
    {
        _logger = logger;
        _contentApiUrl = "https://content-api.wildberries.ru";
        _marketplaceApiUrl = "https://marketplace-api.wildberries.ru";
    }

    public async Task<CardList> GetCardListAsync(GetCardsRequest request)
    {
        try
        {
            var response = await new Url(_contentApiUrl)
                .AppendPathSegment("/content/v2/get/cards/list")
                .WithOAuthBearerToken(_token)
                .PostJsonAsync(request)
                .ReceiveJson<CardList>();

            if (response != null)
            {
                return response;
            }

            throw new Exception("Ошибка получения карточек товаров");
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetAllAssemblyTasks), ex.Message);
            throw;
        }
    }

    public async Task<List<Order>> GetAllAssemblyTasks()
    {
        try
        {
            var url = new Url(_marketplaceApiUrl).AppendPathSegment("/api/v3/orders/new");
            var response = await url.WithOAuthBearerToken(_token).GetJsonAsync<Wrapper<List<Order>>>();
            var result = response.Orders;
            if (result != null)
            {
                return result;
            }
            throw new Exception("Ошибка получения сборочных заданий");
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetAllAssemblyTasks), ex.Message);
            throw;
        }
    }

    public async Task<string> CreateNewSupply(string name)
    {
        try
        {
            var suplyName = new Supply() { Name = name };
            var response = await new Url(_marketplaceApiUrl).AppendPathSegment("/api/v3/supplies")
                                             .WithOAuthBearerToken(_token)
                                             .PostJsonAsync(suplyName)
                                             .ReceiveJson<CreateSupplyResponse>();
            return response.Id;
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(CreateNewSupply), ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{service}.{method} Error while creating supply : {message}",
                nameof(WbClient), nameof(CreateNewSupply), ex.Message);
            throw;
        }
    }

    public async Task<bool> AddOrderToSupplyAsync(string supplyId, long orderId)
    {
        try
        {
            var response = await new Url(_marketplaceApiUrl).AppendPathSegment($"/api/v3/supplies/{supplyId}/orders/{orderId}")
                                             .WithOAuthBearerToken(_token).PatchAsync();
            if (response.StatusCode == 204)
            {
                _logger.LogInformation("Сборочное задание {orderId} успешно добавлено в поставку {supplyId}", orderId, supplyId);
                return true;
            }
            return false;
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "{service}.{method} Error while adding order to supply : {message}",
                nameof(WbClient), nameof(AddOrderToSupplyAsync), ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{service}.{method} Error while adding order to supply : {message}",
                nameof(WbClient), nameof(AddOrderToSupplyAsync), ex.Message);
            throw;
        }
    }

    public async Task<bool> GetWarehouses()
    {
        try
        {
            var url = new Url(_marketplaceApiUrl).AppendPathSegment("/api/v3/warehouses");
            var result = await url.WithOAuthBearerToken(_token).GetJsonAsync<Warehouses[]>();
            return true;
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetWarehouses), ex.Message);
            throw;
        }
    }
    public void LogInfo(string message, string methodName) =>
        _logger.LogInformation("{service}.{method} {message}", nameof(WbClient), methodName, nameof(message));
}
