using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Domain.Models.Crpt.Auth;
using CryptoPro.Security.Cryptography.Pkcs;
using CryptoPro.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace BL.Services;

public sealed class AuthService(ICrptHttpClient httpClient, ILogger<AuthService> logger) : IAuthService
{
    private readonly ICrptHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<AuthService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string? _token = null;
    private DateTime? _tokenExpiration = null;

    public async Task<string> GetTokenAsync()
    {
        if (_token is not null && _tokenExpiration is not null && _tokenExpiration.Value.AddHours(10) > DateTime.Now)
        {
            return _token;
        }

        var data = await _httpClient.GetAuthDataAsync();

        if (data?.Data is null)
        {
            return string.Empty;
        }
        //подписание

        var signData = SignData(Encoding.Default.GetBytes(data.Data), "Быченкова");

        var tokenResponse = await _httpClient.GetTokenAsync(new AuthSignedRequest()
        {
            Uuid = data.Uuid!,
            Data = signData
        });

        if (tokenResponse is not null && tokenResponse.Error is null)
        {
            _tokenExpiration = DateTime.Now;
            _token = tokenResponse.Token;

            return tokenResponse.Token!;
        }
        else
        {
            return string.Empty;
        }
    }

    private CpX509Certificate2? GetCertificate(string signerName)
    {
        var storeMy = new CpX509Store(StoreName.My,
            StoreLocation.CurrentUser);
        storeMy.Open(OpenFlags.ReadOnly);

        var certColl =
            storeMy.Certificates.Find(X509FindType.FindBySubjectName,
                signerName, true);

        if (certColl.Count == 0)
        {
            return null;
        }

        storeMy.Close();

        return certColl[0];
    }

    public string SignData(byte[] msg, string signerName, bool detached = false)
    {
        var sert = GetCertificate(signerName);
        
        var contentInfo = new ContentInfo(msg);
        var signedCms = new CpSignedCms(contentInfo, detached);
        var cmsSigner = new CpCmsSigner(sert);

        signedCms.ComputeSignature(cmsSigner);
        var byteSign = signedCms.Encode();

        return Convert.ToBase64String(byteSign);
    }
}