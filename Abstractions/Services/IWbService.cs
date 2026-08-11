namespace Abstractions.Services;

public interface IWbService
{
    /// <summary>
    /// Создать все поставки из сборочных заданий
    /// </summary>
    /// <returns>Успешность</returns>
    Task<int> CreateDailySupplies();

    /// <summary>
    /// Заполнить существующие поставки новыми сборочными заданиями
    /// </summary>
    Task FillCreatedSupplies();
}
