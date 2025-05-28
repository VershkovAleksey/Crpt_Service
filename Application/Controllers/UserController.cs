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
public class UserController(ICurrentUserService currentUserService, INationalCatalogService nationalCatalogService)
    : ControllerBase
{
    private readonly ICurrentUserService _currentUserService =
        currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

    private readonly INationalCatalogService _nationalCatalogService =
        nationalCatalogService ?? throw new ArgumentNullException(nameof(nationalCatalogService));

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
        try
        {
            var tokenResponse = await _currentUserService.GetToken(authDto.Username, authDto.Password);

            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody, Required] RegisterDto registerDto)
    {
        try
        {
            var response = await _currentUserService.RegisterUser(registerDto);

            await _nationalCatalogService.SeedDataAsync();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}