using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Abstractions.Services;
using Domain.Models.Registration;
using Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(ICurrentUserService currentUserService) : Controller
{
    private readonly ICurrentUserService _currentUserService =
        currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    /// <summary>
    /// Получение токена авторизации
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>Токен</returns>
    [HttpPost("/token")]
    public async Task<IActionResult> Token([FromQuery, Required] string username, [FromQuery, Required] string password)
    {
        var tokenResponse = await _currentUserService.GetToken(username, password);

        return Json(tokenResponse);
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody, Required] RegisterDto registerDto)
    {
        var response = await _currentUserService.RegisterUser(registerDto);

        return Json(response);
    }
}