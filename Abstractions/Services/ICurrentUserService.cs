using System.Security.Claims;
using Database.Entities.Users;
using Domain.Models.Registration;

namespace Abstractions.Services;

public interface ICurrentUserService
{
    Task<TokenResponse> RegisterUser(RegisterDto registerDto);

    Task<TokenResponse> GetToken(string username, string password);

    public void SetCurrentUser(int userId);
    
    public UserEntity CurrentUser { get; }
}