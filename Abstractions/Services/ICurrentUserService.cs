using System.Security.Claims;
using Domain.Models.Registration;

namespace Abstractions.Services;

public interface ICurrentUserService
{
    Task<TokenResponse> RegisterUser(RegisterDto registerDto);

    Task<TokenResponse> GetToken(string username, string password);
}