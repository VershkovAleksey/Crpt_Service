using System.Threading.Tasks;
using Domain.Models.Crpt.Auth;

namespace Abstractions.Services;

public interface IAuthService
{
    public Task<AuthResponseDataDto?> GetAuthDataAsync(int userId, CancellationToken cancellationToken = default);

    // string SignData(byte[] msg, string signerName, bool detached = false);

    public Task<string> GetTokenAsync(AuthSignedRequest signInDto, CancellationToken cancellationToken = default);
}