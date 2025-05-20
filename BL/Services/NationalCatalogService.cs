using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Database.Context;
using Database.Entities.CreateSetRequest;
using Database.Entities.Sets;
using Database.Entities.Units;
using Domain.Models.Crpt.Marking.Enums;
using Domain.Models.NationalCatalog;
using Domain.Models.NationalCatalog.Dto;
using Domain.Models.NationalCatalog.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BL.Services;

public class NationalCatalogService(
    INkHttpClient nkHttpClient,
    ILogger<NationalCatalogService> logger,
    CrptContext crptContext,
    ICurrentUserService currentUserService)
    : INationalCatalogService
{
    private readonly INkHttpClient
        _nkHttpClient = nkHttpClient ?? throw new ArgumentNullException(nameof(nkHttpClient));

    private readonly ILogger<NationalCatalogService>
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    private readonly ICurrentUserService _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    private readonly CrptContext _crptContext = crptContext ?? throw new ArgumentNullException(nameof(crptContext));

    public async Task<GetProductListResponse?> GetProductListAsync(CancellationToken cancellationToken = default)
    {
        return await _nkHttpClient.GetProductListAsync(cancellationToken);
    }

    public async Task<List<SetOptionDto>> GetSetsAsync()
    {
        //TODO:Переделать на currentUserId
        return _crptContext.Sets.Where(x => x.UserId == _currentUserService.CurrentUser.Id).Select(x => new SetOptionDto()
        {
            Id = x.Id,
            SetName = x.SetName,
            Gtin = x.Gtin
        }).ToList();
    }

    public async Task<bool> CreateSetsAsync(IEnumerable<SetOptionDto> options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        try
        {
            //TODO:GetUserId
            var entitiesToInsert = options.Select(x => MapSetRequest(x, _currentUserService.CurrentUser.Id)).ToList();
            await _crptContext.AddRangeAsync(entitiesToInsert, cancellationToken);
            await _crptContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ClassName}.{MethodName} Error while creating sets: {Message}",
                nameof(NationalCatalogService), nameof(CreateSetsAsync), ex.Message);
            throw;
        }

        return true;
    }

    public async Task<List<CreatedSetsDto>> GetSetsByUserIdAsync(int userId) =>
        _crptContext.CreateSetRequests
            .Where(x => x.UserId == userId)
            .ToList()
            .Select(x => MapSetsToDto(x))
            .ToList();

    private CreatedSetsDto MapSetsToDto(CreateSetRequestEntity setEntity)
    {
        return new CreatedSetsDto()
        {
            SetName = setEntity.SetName,
            Status = (CreateSetStatus)setEntity.Status,
            Response = setEntity.Response,
            Count = setEntity.Count,
            Gtin = setEntity.Gtin,
            Id = setEntity.Id,
        };
    }

    private CreateSetRequestEntity MapSetRequest(SetOptionDto setOptionDto, int userId)
    {
        return new CreateSetRequestEntity()
        {
            Id = setOptionDto.Id,
            SetName = setOptionDto.SetName,
            Gtin = setOptionDto.Gtin,
            UserId = userId,
            Count = setOptionDto.Count.Value,
            Status = (int)CreateSetStatus.Proccessed
        };
    }

    public async Task SeedDataAsync(CancellationToken cancellationToken = default)
    {
        var productResponse = await GetProductListAsync(cancellationToken);

        var productDetailInfoList = await GetProductDetailInfoListAsync(productResponse, cancellationToken);

        var kits = await GetKitsAsync(productDetailInfoList, productResponse);
        var sets = await GetSetsAsync(productDetailInfoList, productResponse);

        await _crptContext.Sets.AddRangeAsync(sets, cancellationToken);
        await _crptContext.Sets.AddRangeAsync(kits, cancellationToken);

        await _crptContext.SaveChangesAsync(cancellationToken);

        var units = await GetUnitsAsync(productDetailInfoList, productResponse);

        await _crptContext.Units.AddRangeAsync(units, cancellationToken);

        await _crptContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<ProductDetailInfoResponse>> GetProductDetailInfoListAsync(
        GetProductListResponse productListResponse,
        CancellationToken cancellationToken = default)
    {
        var productDetailInfoList = new List<ProductDetailInfoResponse>();

        foreach (var product in productListResponse.Result.Goods)
        {
            var currentGood = await _nkHttpClient.GetProductDetailInfoAsync(product.Gtin!, cancellationToken);
            productDetailInfoList.Add(currentGood);
        }

        return productDetailInfoList;
    }

    private async Task<List<SetEntity>> GetKitsAsync(List<ProductDetailInfoResponse> productDetailInfoList,
        GetProductListResponse productListResponse) =>
        productDetailInfoList
            .Where(x => x.Result.First().IsKit)
            .Select(productDetail => productDetail.Result.First())
            .Select(kit => new SetEntity
            {
                SetName = kit.GoodName ?? string.Empty,
                Gtin = kit.Gtin ??
                       productListResponse.Result!.Goods!.FirstOrDefault(x => x.GoodName == kit.GoodName)!.Gtin ??
                       string.Empty,
                UserId = _currentUserService.CurrentUser.Id, SetType = SetTypeEnum.Kit
            })
            .ToList();


    private async Task<List<SetEntity>> GetSetsAsync(List<ProductDetailInfoResponse> productDetailInfoList,
        GetProductListResponse productListResponse) =>
        productDetailInfoList
            .Where(x => x.Result.First().IsSet)
            .Select(productDetail => productDetail.Result.First())
            .Select(kit => new SetEntity
            {
                SetName = kit.GoodName ?? string.Empty,
                Gtin = kit.Gtin ??
                       productListResponse.Result!.Goods!.FirstOrDefault(x => x.GoodName == kit.GoodName)!.Gtin ??
                       string.Empty,
                UserId = _currentUserService.CurrentUser.Id, SetType = SetTypeEnum.Set
            })
            .ToList();


    private async Task<List<UnitEntity>> GetUnitsAsync(List<ProductDetailInfoResponse> productDetailInfoList,
        GetProductListResponse productListResponse)
    {
        var unitDetailInfoList = productDetailInfoList
            .Where(x => x.Result.First().IsKit == false && x.Result.First().IsSet == false)
            .ToList();

        var result = new List<UnitEntity>();

        var setEntities = _crptContext.Sets
            .Where(x => x.SetType == SetTypeEnum.Set && x.UserId == _currentUserService.CurrentUser.Id)
            .ToList();

        foreach (var productDetail in unitDetailInfoList)
        {
            var unit = productDetail.Result.First();

            var entity = new UnitEntity()
            {
                Name = unit.GoodName ?? string.Empty,
                Gtin = unit.Gtin ??
                       productListResponse.Result!.Goods!.FirstOrDefault(x => x.GoodName == unit.GoodName)!.Gtin ??
                       string.Empty,
                UserId = _currentUserService.CurrentUser.Id,
            };

            var setsContainingUnit = productDetailInfoList
                .Where(x => x.Result.First().IsSet &&
                            x.Result.First().SetGtins!.Select(s => s.Gtin).Contains(entity.Gtin))
                .ToList();

            var setsFromDb = setsContainingUnit
                .Select(setWithUnit => setEntities
                    .First(x => x.SetName == setWithUnit.Result.First().GoodName ||
                                x.Gtin == setWithUnit.Result.First().Gtin))
                .ToList();

            entity.SetIds = setsFromDb
                .Select(x => x.Id)
                .ToList();

            result.Add(entity);
        }

        return result;
    }
}