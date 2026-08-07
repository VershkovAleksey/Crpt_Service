using Abstractions.Services;
using Domain.Models.Crpt.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WbController : ControllerBase
{
    private readonly IWbService _wbService;

    public WbController(IWbService wbService)
    {
        _wbService = wbService;
    }

    [HttpGet]
    [Route("create-supplies")]
    public async Task<IActionResult> CreateSupplies()
    {
        try
        {
            var res = await _wbService.CreateDailySupplies();
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}