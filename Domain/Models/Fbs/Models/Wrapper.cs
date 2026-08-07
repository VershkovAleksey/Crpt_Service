namespace WbManageBot.Models
{
    /// <summary>
    /// Обертка для получения массивов
    /// </summary>
    /// <typeparam name="T">Обычно List<></typeparam>
    public class Wrapper<T>
    {
        public T Orders { get; set; }
    }   
}
