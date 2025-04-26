using System.Threading.Tasks;
using Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController(IMarkingService markingService, ILogger<TestController> logger)
    : ControllerBase
{
    private readonly IMarkingService _markingService = markingService;
    private readonly ILogger<TestController> _logger = logger;

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Test()
    {
        await _markingService.CreateSetsAsync();
        return Ok();
    }
}