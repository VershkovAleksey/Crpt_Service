using Abstractions.Services;
using BL.Infrastructure.Http;
using Domain.Models.Fbs;
using Domain.Models.Fbs.Models;
using Domain.Models.Fbs.Requests;
using Domain.Models.Fbs.Responses;
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

    /*
     * Надо такой алгоритм: при добавлении из новых на сборку надо чтобы он смотрел уже собранные папки
     * с товарами и докладывал в уже имеющиеся папки в соответствии с цветом и размером и если нет совпадения
     * то ничего бы не делал и потихоньку мы приедем к автоматизации
     */

    public async Task FillCreatedSupplies()
    {
        //1. GET /api/v3/supplies - получаем список поставок существующих
        //2. Получаем все, берем те, у которых Done = false - открытые поставки на сборке
        //3. Берем их айдишники
        var supplyIds = await GetSupplyIdsListAsync();

        // Получаем список новых сборочных заданий - типа новые заказы
        var assemblies = await _client.GetNewAssemblyTasks();

        //Получаем список сборочных заданий - все заказы, которые уже в поставке, доставке, новые и т.д.
        var ordersList = await GetOrdersListAsync();

        await ProcessExistSuppliesAsync(supplyIds, ordersList, assemblies);
    }

    public async Task<int> CreateDailySupplies()
    {
        try
        {
            //Создаем переменные для карточек товаров и запроса на получение карточек
            CardList cardList = new();
            GetCardsRequest getCardsRequest = new();

            await GetCardListAsync(getCardsRequest, cardList);

            // Получаем список новых сборочных заданий
            var assemblies = await _client.GetNewAssemblyTasks();

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


    private async Task<List<Order>> GetOrdersListAsync()
    {
        var ordersList = new GetOrdersListResponse();
        var request = new GetWithLimitRequest();
        int ordersCount;

        do
        {
            var currentResponse = await _client.GetOrdersAsync(request);

            ordersCount = currentResponse.Orders.Count;
            ordersList.Orders.AddRange(currentResponse.Orders);
            request.Next = currentResponse.Next;
        } while (ordersCount == request.Limit);

        return ordersList.Orders;
    }

    private async Task ProcessExistSuppliesAsync(List<string> supplyIds, List<Order> ordersList, List<Order> assemblies)
    {
        foreach (var supplyId in supplyIds)
        {
            //Получаем идентификаторы сборочных заданий(заказов) в поставке
            var orderInSupply = await _client.GetOrderIdsFromSupplyAsync(supplyId);

            if (orderInSupply.OrderIds == null || orderInSupply.OrderIds.Count == 0)
            {
                _logger.LogInformation(
                    "{service}.{method} Не найдено заказов в поставке {supplyId}",
                    nameof(WbService), nameof(IWbService.CreateDailySupplies), supplyId);
                continue;
            }

            //Берем из списка сборочных заданий(заказов) любое задание из поставки
            var order = ordersList.FirstOrDefault(x => orderInSupply.OrderIds.Any(orderId => x.Id == orderId));
            if (order is null)
            {
                continue;
            }

            //Выбираем из новых заказов те, что подходят к поставке по товару и СЦ
            // chrtId - размер\цвет, WarehouseId - идентификатор сортировчного центра
            var assembliesToSupply = assemblies.Where(x => x.ChrtId == order.ChrtId && x.WarehouseId == order.WarehouseId).ToList();

            var request = new AddOrdersToSupplyRequest()
            {
                Orders = assembliesToSupply.Select(x => x.Id).ToList()
            };

            //Добавляем заказы в поставку
            var isSuccess = await _client.AddOrdersToSupplyAsync(supplyId, request);

            if (!isSuccess)
            {
                _logger.LogWarning(
                    "{service}.{method} problem while adding order {orderId} to supply {supplyId}",
                    nameof(WbService), nameof(IWbService.CreateDailySupplies),
                    JsonConvert.SerializeObject(request), supplyId);
            }

            _logger.LogInformation("Успешно добавлено {count} сборочных заданий в поставку {name}",
                request.Orders.Count, supplyId);
        }
    }

    private async Task<List<string>> GetSupplyIdsListAsync()
    {
        var suppliesList = new GetSuppliesListResponse();
        var request = new GetWithLimitRequest();
        int suppliesCount;
        do
        {
            var currentResponse = await _client.GetSuppliesListAsync(request);

            suppliesCount = currentResponse.Supplies.Count;
            suppliesList.Supplies.AddRange(currentResponse.Supplies);
            request.Next = currentResponse.Next;
        } while (suppliesCount == request.Limit);

        return suppliesList.Supplies.Where(x => !x.Done).Select(x => x.Id).ToList();
    }


    private async Task<int> ProcessOrdersAsync(List<List<Order>> sortedOrders, CardList cardList)
    {
        var supplyCount = 0;

        //Формируем поставки исходя из сортированных сборочных заданий (заказов)
        foreach (var orders in sortedOrders)
        {
            try
            {
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

                if (!isSuccess)
                {
                    _logger.LogWarning(
                        "{service}.{method} problem while adding order {orderId} to supply {supplyId}",
                        nameof(WbService), nameof(IWbService.CreateDailySupplies),
                        JsonConvert.SerializeObject(request), supplyId);
                }

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