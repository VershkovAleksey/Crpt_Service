using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using CryptoPro.Security.Cryptography.X509Certificates;
using Domain.Models.Crpt.Auth;
using Domain.Models.Crpt.Marking.Dto;
using Domain.Models.Crpt.Marking.Request;
using Domain.Models.NationalCatalog.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController(
    IMarkingService markingService,
    ILogger<TestController> logger,
    INkHttpClient nkHttpClient,
    INationalCatalogService nationalCatalogService,
    IAuthService authService)
    : ControllerBase
{
    private readonly IMarkingService _markingService = markingService;
    private readonly IAuthService _authService = authService;
    private readonly INationalCatalogService _nationalCatalogService = nationalCatalogService;
    private readonly INkHttpClient _inkHttpClient = nkHttpClient;
    private readonly ILogger<TestController> _logger = logger;

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Test()
    {
        await _nationalCatalogService.SeedDataAsync();
        return Ok();
    }

    [HttpGet]
    [Route("sets")]
    public async Task<IActionResult> GetSetsAsync()
    {
        var res = await _nationalCatalogService.GetSetsAsync();
        return Ok(res);
    }

    [HttpPost]
    [Route("createSets")]
    public async Task<IActionResult> CreateSetsAsync([FromBody, Required] IEnumerable<SetOptionDto> sets)
    {
        var res = await _nationalCatalogService.CreateSetsAsync(sets);
        return Ok();
    }

    [HttpGet]
    [Route("auth-data")]
    public async Task<IActionResult> GetAuthDataAsync()
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Value == "UserId");
        var res = await _authService.GetAuthDataAsync(1);
        return Ok(res);
    }

    [HttpPost]
    [Route("crpt-token")]
    public async Task<IActionResult> GetTokenAsync(AuthSignedRequest request)
    {
        ;
        // _logger.LogInformation(request.Data.Replace("\r\n",string.Empty));
        //request.Data = request.Data.Replace("\r\n", string.Empty);
        var token = await _authService.GetTokenAsync(request);
        return Ok(token);
    }

    [HttpPost]
    [Route("send-sets-to-mark")]
    public async Task<IActionResult> SendSetsToMarkAsync([FromBody, Required] SendSetsDto request)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Value == "UserId");
        await _markingService.SignDataAsync(request.Token, 1, request.Request);
        return Ok();
    }

    [HttpPost]
    [Route("data-to-sign")]
    public async Task<IActionResult> GetSignDataAsync([FromHeader(Name = "token"), Required] string token)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Value == "UserId");
        var res = await _markingService.CreateSetsAsync(token, 1);
        return Ok(res);
    }
}