using MarketParserNet.Framework.Interface;
using MarketParserNet.Utils;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    ///     Конфигурация Redis
    /// </summary>
    public class FileConfigurationRedis : IRedisConfig
    {
        // TODO выделить конфигурацию Redis в отдельную секцию

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за строку подключения к Redis
        /// </summary>
        public const string ConnectionStringKey = "RedisConnectionString";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за количество попыток подключения к Redis
        /// </summary>
        public const string RetryCountKey = "RedisRetryCount";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за паузу перед попытками подключения к Redis
        /// </summary>
        public const string ReconnectIntervalKey = "RedisReconnectInterval";

        /// <summary>
        ///     Строка подключения
        /// </summary>
        public string ConnectionString => ConfigurationHelper.AppSetting(ConnectionStringKey, "localhost");

        /// <summary>
        ///     Количество попыток подключения
        /// </summary>
        public int RetryCount => ConfigurationHelper.AppSettingInt(RetryCountKey, 1);

        /// <summary>
        ///     Интервал переподключения
        /// </summary>
        public int ReconnectInterval => ConfigurationHelper.AppSettingInt(ReconnectIntervalKey, 30000);
    }
}