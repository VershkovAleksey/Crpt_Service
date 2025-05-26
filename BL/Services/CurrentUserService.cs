using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Database.Context;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Abstractions.Services;
using Database.Entities.Users;
using Domain.Models.Registration;
using Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BL.Services;

public class CurrentUserService(
    CrptContext dbContext,
    ILogger<CurrentUserService> logger,
    INationalCatalogService nationalCatalogService) : ICurrentUserService
{
    private readonly CrptContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly INationalCatalogService _nationalCatalogService =
        nationalCatalogService ?? throw new ArgumentNullException(nameof(nationalCatalogService));

    private readonly ILogger<CurrentUserService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public UserEntity CurrentUser { get; private set; }

    public async Task<TokenResponse> RegisterUser(RegisterDto registerDto)
    {
        var newUser = new UserEntity()
        {
            Email = registerDto.Email ?? throw new ArgumentNullException("Email is required"),
            FirstName = registerDto.FirstName ?? throw new ArgumentNullException("First name is required"),
            LastName = registerDto.LastName ?? throw new ArgumentNullException("Last name is required"),
            PasswordHash = ComputeHash(registerDto.Password ?? throw new ArgumentNullException("Password is required"),
                new MD5CryptoServiceProvider()),
            Login = registerDto.Email ?? throw new ArgumentNullException("Email is required"),
            Role = "User",
            PhoneNumber = registerDto.PhoneNumber,
            NkApiKey = registerDto.ApiKey ?? throw new ArgumentNullException("Api key is required"),
        };

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        CurrentUser = await _dbContext.Users.FirstAsync(x => x.Login == newUser.Login && x.PasswordHash == newUser.PasswordHash);

        await _nationalCatalogService.SeedDataAsync();
        
        return (await GetToken(registerDto.Email, registerDto.Password))!;
    }

    public async Task<TokenResponse> GetToken(string username, string password)
    {
        var identity = await GetIdentity(username, password);
        if (identity == null)
        {
            throw new UnauthorizedAccessException();
        }

        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new TokenResponse()
        {
            AccessToken = encodedJwt,
            UserName = identity.Name
        };
    }

    /// <summary>
    /// Получение токена
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль/param>
    private async Task<ClaimsIdentity?> GetIdentity(string username, string password)
    {
        string hPassword = ComputeHash(password, new MD5CryptoServiceProvider());

        var user = _dbContext.Users.FirstOrDefault(x =>
            (x.Login == username || x.Email == username) && x.PasswordHash == hPassword);

        if (user == null)
            return null;

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.FirstName + " " + user.LastName),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role ?? "User"),
            new("UserId", user.Id.ToString()),
            new("Inn", user.Inn ?? string.Empty),
            new("ApiKey", user.NkApiKey ?? string.Empty)
        };

        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;

        // если пользователя не найдено
    }

    private string ComputeHash(string input, HashAlgorithm algorithm)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);

        var hashedBytes = algorithm.ComputeHash(inputBytes);

        return BitConverter.ToString(hashedBytes);
    }

    public void SetCurrentUser(int userId)
    {
        var currentUser = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
        if (currentUser is not null)
        {
            CurrentUser = currentUser;
        }
        else
        {
            throw new Exception("Пользователь с таким Id не найден");
        }
    }
}