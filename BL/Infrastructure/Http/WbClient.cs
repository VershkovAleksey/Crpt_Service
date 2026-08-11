using Domain.Models.Fbs;
using Domain.Models.Fbs.Models;
using Domain.Models.Fbs.Requests;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using Domain.Models.Fbs.Responses;
using WbManageBot.Models;

namespace BL.Infrastructure.Http;

/// <summary>
/// HTTP-клиент для взаимодействия с Wildberries openApi
/// </summary>
public class WbClient(ILogger<WbClient> logger)
{
    //Тест
    //private const string Token = "eyJhbGciOiJFUzI1NiIsImtpZCI6IjIwMjYwMzAydjEiLCJ0eXAiOiJKV1QifQ.eyJhY2MiOjIsImVudCI6MSwiZXhwIjoxODAxOTA0MTAyLCJpZCI6IjAxOWZkZTAyLTA1NjUtNzcxZS1hMzhmLWMxM2JhZmEzNTVlNiIsImlpZCI6MjcwODIxNTUsIm9pZCI6MTI4ODMzMiwicyI6MCwic2lkIjoiYjE4M2M0MGEtNzk1Yi00NDcyLThhNDktNGIzMmZjMmY1MTYzIiwidCI6dHJ1ZSwidWlkIjoyNzA4MjE1NX0.AVHs4Fs2DSrkE0Ul_5b_JX0-P7flKqwCHIo5YWVLmF9eQx__DNFi1Gc9dS-_r0xNfA5UDWGeydUGvpsJpQWjOQ";

    //Прод
    private const string Token = "eyJhbGciOiJFUzI1NiIsImtpZCI6IjIwMjYwMzAydjEiLCJ0eXAiOiJKV1QifQ.eyJhY2MiOjEsImVudCI6MSwiZXhwIjoxODAxOTAxMTk1LCJpZCI6IjAxOWZkZGQ1LWFhM2ItNzQzNy1iY2VhLTg0NTY1YWM3OTJjMyIsImlpZCI6MjcwODIxNTUsIm9pZCI6MTI4ODMzMiwicyI6MTgsInNpZCI6ImIxODNjNDBhLTc5NWItNDQ3Mi04YTQ5LTRiMzJmYzJmNTE2MyIsInQiOmZhbHNlLCJ1aWQiOjI3MDgyMTU1fQ.MXvzrN4_2ApQCsQznqse_mhEsOCCujcjDRi2gbr3n-S2IZFPewPPBr2Jzwdkd3zms1-P3aCec94Ir1_NsFRrlQ";

    private const string ContentApiUrl = "https://content-api.wildberries.ru";
    private const string MarketplaceApiUrl = "https://marketplace-api.wildberries.ru";

    public async Task<GetOrderIdsFromSupplyResponse> GetOrderIdsFromSupplyAsync(string supplyId)
    {
        try
        {
            var response = await new Url(MarketplaceApiUrl)
                .AppendPathSegment($"/api/marketplace/v3/supplies/{supplyId}/order-ids")
                .WithOAuthBearerToken(Token).GetJsonAsync<GetOrderIdsFromSupplyResponse>();

            return response ?? throw new Exception("Ошибка получения списка сборочных заданий в поставке");
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetNewAssemblyTasks), ex.Message);
            throw;
        }
    }
    
    public async Task<GetOrdersListResponse> GetOrdersAsync(GetWithLimitRequest getWithLimitRequest)
    {
        try
        {
            var response = await new Url(MarketplaceApiUrl)
                .AppendPathSegment($"/api/v3/orders")
                .WithOAuthBearerToken(Token)
                .SetQueryParams(getWithLimitRequest)
                .GetJsonAsync<GetOrdersListResponse>();

            return response ?? throw new Exception("Ошибка получения списка сборочных заданий");
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetNewAssemblyTasks), ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Метод возвращает список поставок.
    /// </summary>
    /// <param name="getWithLimitRequest">Запрос на получение списка поставок</param>
    /// <returns>Список поставок</returns>
    public async Task<GetSuppliesListResponse> GetSuppliesListAsync(GetWithLimitRequest getWithLimitRequest)
    {
        try
        {
            var response = await new Url(MarketplaceApiUrl)
                .AppendPathSegment("/api/v3/supplies")
                .SetQueryParams(getWithLimitRequest)
                .WithOAuthBearerToken(Token)
                .GetJsonAsync<GetSuppliesListResponse>();

            return response ?? throw new Exception("Ошибка получения списка поставок");
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetNewAssemblyTasks), ex.Message);
            throw;
        }
    }

    public async Task<CardList> GetCardListAsync(GetCardsRequest request)
    {
        try
        {
            var response = await new Url(ContentApiUrl)
                .AppendPathSegment("/content/v2/get/cards/list")
                .WithOAuthBearerToken(Token)
                .PostJsonAsync(request)
                .ReceiveJson<CardList>();

            return response ?? throw new Exception("Ошибка получения карточек товаров");
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetNewAssemblyTasks), ex.Message);
            throw;
        }
    }

    public async Task<List<Order>> GetNewAssemblyTasks()
    {
        try
        {
            var url = new Url(MarketplaceApiUrl).AppendPathSegment("/api/v3/orders/new");
            var response = await url.WithOAuthBearerToken(Token).GetJsonAsync<Wrapper<List<Order>>>();
            var result = response.Orders;

            return result ?? throw new Exception("Ошибка получения сборочных заданий");
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetNewAssemblyTasks), ex.Message);
            throw;
        }
    }

    public async Task<string> CreateNewSupply(string name)
    {
        try
        {
            var supply = new Supply() { Name = name };
            var response = await new Url(MarketplaceApiUrl).AppendPathSegment("/api/v3/supplies")
                .WithOAuthBearerToken(Token)
                .PostJsonAsync(supply)
                .ReceiveJson<CreateSupplyResponse>();

            return response.Id;
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(CreateNewSupply), ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{service}.{method} Error while creating supply : {message}",
                nameof(WbClient), nameof(CreateNewSupply), ex.Message);
            throw;
        }
    }

    public async Task<bool> AddOrdersToSupplyAsync(string supplyId, AddOrdersToSupplyRequest request)
    {
        try
        {
            var response = await new Url(MarketplaceApiUrl)
                .AppendPathSegment($"api/marketplace/v3/supplies/{supplyId}/orders")
                .WithOAuthBearerToken(Token).PatchJsonAsync(request);

            if (response.StatusCode != 204)
            {
                return false;
            }

            logger.LogInformation("Сборочное задания {orderId} успешно добавлено в поставку {supplyId}",
                JsonConvert.SerializeObject(request), supplyId);
            return true;
        }
        catch (FlurlHttpException ex)
        {
            var error = ex.Message;

            if (ex.StatusCode == (int)HttpStatusCode.Conflict)
            {
                var resp = await ex.Call.Response.GetStringAsync();
                error += resp;
            }

            logger.LogError(ex, "{service}.{method} Error while adding order to supply : {message}",
                nameof(WbClient), nameof(AddOrdersToSupplyAsync), error);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{service}.{method} Error while adding order to supply : {message}",
                nameof(WbClient), nameof(AddOrdersToSupplyAsync), ex.Message);
            throw;
        }
    }

    public async Task<bool> GetWarehouses()
    {
        try
        {
            var url = new Url(MarketplaceApiUrl).AppendPathSegment("/api/v3/warehouses");
            var result = await url.WithOAuthBearerToken(Token).GetJsonAsync<Warehouses[]>();
            return true;
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{service}.{method} : {message}",
                nameof(WbClient), nameof(GetWarehouses), ex.Message);
            throw;
        }
    }
}