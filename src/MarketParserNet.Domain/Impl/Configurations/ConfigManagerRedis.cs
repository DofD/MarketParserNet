using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    ///     Менеджер конфигурации Redis
    /// </summary>
    public class ConfigManagerRedis : IConfigManager<IRedisConfig>
    {
        /// <summary>
        ///     Получить конфигурацию
        /// </summary>
        /// <returns>Конфигурация</returns>
        public IRedisConfig GetConfiguration()
        {
            return new FileConfigurationRedis();
        }
    }
}