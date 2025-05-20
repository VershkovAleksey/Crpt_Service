using System.ComponentModel.DataAnnotations;
using Abstractions.Services;
using Domain.Models.NationalCatalog.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NationalCatalogController(
    INationalCatalogService nationalCatalogService,
    ICurrentUserService currentUserService)
    : ControllerBase
{
    private readonly INationalCatalogService _nationalCatalogService =
        nationalCatalogService ?? throw new ArgumentNullException(nameof(nationalCatalogService));

    private readonly ICurrentUserService _currentUserService =
        currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    [HttpGet]
    [Route("seed-data")]
    public async Task<IActionResult> SeedData()
    {
        try
        {
            await _nationalCatalogService.SeedDataAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("set-options")]
    public async Task<IActionResult> GetSetsOptionsAsync()
    {
        try
        {
            var res = await _nationalCatalogService.GetSetsAsync();
            return Ok(res);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    [Route("create-sets")]
    public async Task<IActionResult> CreateSetsAsync([FromBody, Required] IEnumerable<SetOptionDto> sets)
    {
        try
        {
            await _nationalCatalogService.CreateSetsAsync(sets);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    [Route("get-sets")]
    public async Task<IActionResult> GetSetsAsync()
    {
        try
        {
            var res = await _nationalCatalogService.GetSetsByUserIdAsync(_currentUserService.CurrentUser.Id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}