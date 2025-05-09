using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Domain.Settings;

public class AuthOptions
{
    public const string ISSUER = "localhost:5274"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    const string KEY = "mysupersecret_secretkey!123456789";   // ключ для шифрации
    public const int LIFETIME = 100; // время жизни токена - 1 минута
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}