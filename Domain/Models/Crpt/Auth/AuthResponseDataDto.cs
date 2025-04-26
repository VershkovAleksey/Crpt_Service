namespace Domain.Models.Crpt.Auth;

public class AuthResponseDataDto
{
    /// <summary>
    /// Уникальный идентификатор сгенерированных случайных данных
    /// </summary>
    public string? Uuid { get; set; }
    
    /// <summary>
    /// Случайная строка данных
    /// </summary>
    public string? Data { get; set; }
}