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
public class WbService : IWbService
{
    private readonly ILogger<WbService> _logger;
    private readonly WbClient _client;

    public WbService(ILogger<WbService> logger, WbClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    async Task<int> IWbService.CreateDailySupplies()
    {
        try
        {
            CardList cardList = new();
            GetCardsRequest getCardsRequest = new();
            do
            {
                var res = await _client.GetCardListAsync(getCardsRequest);

                cardList.Cursor = res.Cursor;
                cardList.Cards.AddRange(res.Cards);

                getCardsRequest.Settings.Cursor.NmID = res.Cursor.NmID;
                getCardsRequest.Settings.Cursor.UpdatedAt = res.Cursor.UpdatedAt;
            }
            while (cardList.Cursor.Total >= 100);

            var assemblies = await _client.GetAllAssemblyTasks();
            var sizes = await GetSizeListAsync(assemblies);
            var sortedOrders = await GetSortedOrdersByChrtIdAsync(assemblies, sizes);
            var supplyCount = 0;
            var successOrderToSuppliesCount = 0;

            foreach (var orders in sortedOrders)
            {
                try
                {
                    if (successOrderToSuppliesCount + orders.Count >= 300)
                    {
                        _logger.LogInformation("Достигнут лимит кол-ва запросов ждем 1 минуту");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        successOrderToSuppliesCount = 0;
                    }

                    var supplyName = GetSupplyName(cardList, (int)orders.First().ChrtId);
                    var supplyId = await _client.CreateNewSupply(supplyName);

                    var request = new AddOrdersToSupplyRequest()
                    {
                        Orders = orders.Select(x => x.Id).ToList()
                    };

                    var isSuccess = await _client.AddOrdersToSupplyAsync(supplyId, request);

                    await Task.Delay(200);

                    if (!isSuccess)
                    {
                        _logger.LogWarning("{service}.{method} problem while adding order {orderId} to supply {supplyId}",
                            nameof(WbService), nameof(IWbService.CreateDailySupplies), JsonConvert.SerializeObject(request), supplyId);
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

                    continue;
                }
            }
            return supplyCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{service}.{method} Error while creating daily supplies {message}",
                nameof(WbService), nameof(IWbService.CreateDailySupplies), ex.Message);
            throw;
        }
    }

    private async Task<List<int>> GetSizeListAsync(List<Order> orders)
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

    private async Task<List<List<Order>>> GetSortedOrdersByChrtIdAsync(List<Order> orders, List<int> sizes)
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
