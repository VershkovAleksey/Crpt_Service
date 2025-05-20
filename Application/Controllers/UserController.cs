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
public class UserController(ICurrentUserService currentUserService) : ControllerBase
{
    private readonly ICurrentUserService _currentUserService =
        currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    /// <summary>
    /// Получение токена авторизации
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <returns>Токен</returns>
    [HttpPost]
    [Route("auth")]
    public async Task<IActionResult> Token([FromBody, Required] AuthDto authDto)
    {
        var tokenResponse = await _currentUserService.GetToken(authDto.Username, authDto.Password);

        return Ok(tokenResponse);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody, Required] RegisterDto registerDto)
    {
        var response = await _currentUserService.RegisterUser(registerDto);

        return Ok(response);
    }
}