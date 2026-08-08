using Abstractions.Services;
using BL.Infrastructure.Http;
using Domain.Models.Fbs;
using Domain.Models.Fbs.Models;
using Domain.Models.Fbs.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BL.Services;

/// <summary>
/// Сервис логики и манипуляций апи WB. Инкапсулирует в себе бизнес логику. Взаимодействует с клиентом и ботом
/// </summary>
public class WbService(ILogger<WbService> logger, WbClient client) : IWbService
{
    private readonly ILogger<WbService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly WbClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<int> CreateDailySupplies()
    {
        try
        {
            //Создаем переменные для карточек товаров и запроса на получение карточек
            CardList cardList = new();
            GetCardsRequest getCardsRequest = new();

            await GetCardListAsync(getCardsRequest, cardList);

            // Получаем список новых сборочных заданий
            var assemblies = await _client.GetAllAssemblyTasks();

            //Получаем размеры из сборочных заданий
            var sizes = GetSizeList(assemblies);

            //Сортируем сортированные сборочные заданий по размерам и сортировочным центрам
            var sortedOrders = GetSortedOrdersByChrtIdAndSortingCenters(assemblies, sizes);

            var supplyCount = await ProcessOrdersAsync(sortedOrders, cardList);

            return supplyCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{service}.{method} Error while creating daily supplies {message}",
                nameof(WbService), nameof(IWbService.CreateDailySupplies), ex.Message);
            throw;
        }
    }

    private async Task<int> ProcessOrdersAsync(List<List<Order>> sortedOrders, CardList cardList)
    {
        var supplyCount = 0;
        var successOrderToSuppliesCount = 0;

        //Формируем поставки исходя из сортированных сборочных заданий (заказов)
        foreach (var orders in sortedOrders)
        {
            try
            {
                //Проверяем не выжрали ли лимит по запросам (300 в минуту). Если выжрали, ждем 1 минуту и скидываем счетчик
                if (successOrderToSuppliesCount + orders.Count >= 300)
                {
                    _logger.LogInformation("Достигнут лимит кол-ва запросов ждем 1 минуту");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    successOrderToSuppliesCount = 0;
                }

                //Получаем название поставки. Формируется из артикула вб и размера
                var supplyName = GetSupplyName(cardList, (int)orders.First().ChrtId);

                //Создаем новую поставку
                var supplyId = await _client.CreateNewSupply(supplyName);

                var request = new AddOrdersToSupplyRequest()
                {
                    Orders = orders.Select(x => x.Id).ToList()
                };

                //Добавляем заказы в поставку
                var isSuccess = await _client.AddOrdersToSupplyAsync(supplyId, request);

                //Между каждым запросом ждем 200мс (ограничение вб апи)
                await Task.Delay(200);

                if (!isSuccess)
                {
                    _logger.LogWarning(
                        "{service}.{method} problem while adding order {orderId} to supply {supplyId}",
                        nameof(WbService), nameof(IWbService.CreateDailySupplies),
                        JsonConvert.SerializeObject(request), supplyId);
                }

                successOrderToSuppliesCount += request.Orders.Count;

                supplyCount++;
                _logger.LogInformation("Успешно сформировано {count} сборочных заданий в поставку {name}",
                    request.Orders.Count, supplyId);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("{service}.{method} Error while creating supply: {message}",
                    nameof(WbService), nameof(IWbService.CreateDailySupplies), ex.Message);
            }
        }

        return supplyCount;
    }

    private async Task GetCardListAsync(GetCardsRequest getCardsRequest, CardList cardList)
    {
        // Получаем первый пак карточек лимит 100, но бывает приходит по 10. Запрашиваем пока total не будет меньше лимита
        do
        {
            var res = await _client.GetCardListAsync(getCardsRequest);

            cardList.Cursor = res.Cursor;
            cardList.Cards.AddRange(res.Cards);

            getCardsRequest.Settings.Cursor.NmID = res.Cursor.NmID;
            getCardsRequest.Settings.Cursor.UpdatedAt = res.Cursor.UpdatedAt;
            //Дальше получаем пока не выжрем лимит
        } while (cardList.Cursor.Total >= 100);
    }

    private List<int> GetSizeList(List<Order> orders)
    {
        List<int> sizes = new();
        foreach (var order in orders)
        {
            if (!sizes.Contains((int)order.ChrtId))
            {
                sizes.Add((int)order.ChrtId);
            }
        }

        return sizes;
    }

    private List<List<Order>> GetSortedOrdersByChrtIdAndSortingCenters(List<Order> orders, List<int> sizes)
    {
        List<List<Order>> sortedOrders = new();
        foreach (var size in sizes)
        {
            var sizeOrders = orders.Where(x => (int)x.ChrtId == size).ToList();
            if (sizeOrders.Count == 0)
            {
                _logger.LogWarning($"Для размера {size} не найдено сборочных заданий");
            }

            var warehouseIds = sizeOrders.Select(x => x.WarehouseId).Distinct().ToList();

            if (warehouseIds.Count > 1)
            {
                foreach (var id in warehouseIds)
                {
                    sortedOrders.Add(sizeOrders.Where(x => x.WarehouseId == id).ToList());
                }
            }
            else
            {
                sortedOrders.Add(sizeOrders);
            }
        }

        return sortedOrders;
    }

    private string GetSupplyName(CardList cardList, int chrtId)
    {
        var card = cardList.Cards.FirstOrDefault(x => x.Sizes.Any(s => s.ChrtId == chrtId));
        if (card is null)
        {
            throw new Exception("Can not find size in card or card is not exist");
        }

        var size = card.Sizes.First(x => x.ChrtId == chrtId);

        return $"{card.VendorCode} {size.TechSize}";
    }
}