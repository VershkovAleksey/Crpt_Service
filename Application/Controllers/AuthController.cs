using Abstractions.Services;
using Domain.Models.Crpt.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    [HttpGet]
    [Route("auth-data")]
    public async Task<IActionResult> GetAuthDataAsync()
    {
        try
        {
            var res = await _authService.GetAuthDataAsync(_currentUserService.CurrentUser.Id);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("crpt-token")]
    public async Task<IActionResult> GetTokenAsync(AuthSignedRequest request)
    {
        try
        {
            var token = await _authService.GetTokenAsync(request);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}