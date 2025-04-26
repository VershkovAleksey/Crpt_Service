namespace Abstractions.Services;

public interface IAuthService
{
    public Task<string> GetTokenAsync();

    string SignData(byte[] msg, string signerName, bool detached = false);
}