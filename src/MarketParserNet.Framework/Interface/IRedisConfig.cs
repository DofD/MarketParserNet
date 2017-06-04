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
        string ConnectionString { get; }

        /// <summary>
        /// Количество попыток подключения
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// Интервал переподключения
        /// </summary>
        int ReconnectInterval { get; }
    }
}