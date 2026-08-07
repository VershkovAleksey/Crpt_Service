namespace Abstractions.Services;

public interface IWbService
{
    /// <summary>
    /// Создать все поставки из сборочных заданий
    /// </summary>
    /// <returns>Успешность</returns>
    Task<int> CreateDailySupplies();
}
