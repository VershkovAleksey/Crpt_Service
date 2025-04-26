using Newtonsoft.Json;

namespace Domain.Models.Crpt.Auth;

public class AuthSignInResponseDto
{
    /// <summary>
    /// Аутентификационный токен
    /// <remarks>Параметр указывается в случае успешного ответа</remarks>
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Код ошибки
    /// <remarks>Параметр указывается в случае не успешного ответа</remarks>
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    [JsonProperty("error_message")]
    public string? Error { get; set; }
    
    /// <summary>
    /// Описание ошибки
    /// </summary>
    [JsonProperty("description")]
    public string? ErrorDescription { get; set; }
}