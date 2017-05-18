namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Менеджер конфигурации
    /// </summary>
    /// <typeparam name="T">Тип конфигурации</typeparam>
    public interface IConfigManager<out T>
    {
        /// <summary>
        ///     Получить конфигурацию
        /// </summary>
        /// <returns>Конфигурация</returns>
        T GetConfiguration();
    }
}