using System.ComponentModel.DataAnnotations;
using Abstractions.Services;
using Domain.Models.Crpt.Marking.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MarkingController : ControllerBase
{
    private readonly IMarkingService _markingService;
    private readonly ICurrentUserService _currentUserService;

    public MarkingController(IMarkingService markingService,
        ICurrentUserService currentUserService)
    {
        _markingService = markingService ?? throw new ArgumentNullException(nameof(markingService));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    [HttpPost]
    [Route("send-sets-to-mark")]
    public async Task<IActionResult> SendSetsToMarkAsync([FromBody, Required] SendSetsDto request)
    {
        try
        {
            await _markingService.SignDataAsync(request.Token, _currentUserService.CurrentUser.Id, request.Request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("data-to-sign")]
    public async Task<IActionResult> GetSignDataAsync([FromBody, Required] signDataRequest token)
    {
        try
        {
            var res = await _markingService.CreateSetsAsync(token.Token, _currentUserService.CurrentUser.Id);
            return Ok(res);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

public class signDataRequest
{
    public string Token { get; set; }
}