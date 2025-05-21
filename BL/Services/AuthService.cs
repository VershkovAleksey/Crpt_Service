using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Abstractions.Infrastructure.Http;
using Abstractions.Services;
using Domain.Models.Crpt.Auth;
// using CryptoPro.Security.Cryptography.Pkcs;
// using CryptoPro.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace BL.Services;

public sealed class AuthService(ICrptHttpClient httpClient, ILogger<AuthService> logger) : IAuthService
{
    private readonly ICrptHttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<AuthService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string? _token = null;
    private DateTime? _tokenExpiration = null;

    public async Task<AuthResponseDataDto?> GetAuthDataAsync(int userId, CancellationToken cancellationToken = default)
    {
        var data = await _httpClient.GetAuthDataAsync(cancellationToken);

         //var signData = SignData(Encoding.Default.GetBytes(data.Data), "Быченкова");
        //
        // var tokenResponse = await _httpClient.GetTokenAsync(new AuthSignedRequest()
        // {
        //     Uuid = data.Uuid!,
        //     Data = signData
        // });
        //
        // _logger.LogInformation(signData);
        return data?.Data is null ? null : data;
    }

    public async Task<string> GetTokenAsync(AuthSignedRequest signInDto,
        CancellationToken cancellationToken = default)
    {
        var tokenResponse = await _httpClient.GetTokenAsync(signInDto);
        if (tokenResponse is null || !string.IsNullOrWhiteSpace(tokenResponse.Error))
            throw new Exception(tokenResponse.Error + " " + tokenResponse.ErrorDescription);

        if (tokenResponse?.Token is null)
            throw new Exception("Token is null");
        
        return tokenResponse?.Token!;
    }

    // private CpX509Certificate2? GetCertificate(string signerName)
    // {
    //     var storeMy = new CpX509Store(StoreName.My,
    //         StoreLocation.CurrentUser);
    //     storeMy.Open(OpenFlags.ReadOnly);
    //
    //     var certColl =
    //         storeMy.Certificates.Find(X509FindType.FindBySubjectName,
    //             signerName, true);
    //
    //     if (certColl.Count == 0)
    //     {
    //         return null;
    //     }
    //
    //     storeMy.Close();
    //
    //     return certColl[0];
    // }
    //
    // public string SignData(byte[] msg, string signerName, bool detached = false)
    // {
    //     var sert = GetCertificate(signerName);
    //
    //     var contentInfo = new ContentInfo(msg);
    //     var signedCms = new CpSignedCms(contentInfo, detached);
    //     var cmsSigner = new CpCmsSigner(sert);
    //
    //     signedCms.ComputeSignature(cmsSigner);
    //     var byteSign = signedCms.Encode();
    //
    //     return Convert.ToBase64String(byteSign);
    // }
}