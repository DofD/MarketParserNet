namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    /// Конфигурация Redis
    /// </summary>
    public interface IRedisConfig
    {
        /// <summary>
        /// Строка подключения
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Количество попыток подключения
        /// </summary>
        int RetryCount { get; set; }

        /// <summary>
        /// Интервал переподключения
        /// </summary>
        int ReconnectInterval { get; set; }
    }
}