using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    ///     Менеджер конфигурации RabbitMQ
    /// </summary>
    public class ConfigManagerRabbitMQ : IConfigManager<IConfigurationRabbitMq>
    {
        /// <summary>
        ///     Получить конфигурацию
        /// </summary>
        /// <returns>Конфигурация</returns>
        public IConfigurationRabbitMq GetConfiguration()
        {
            return new FileConfigurationRabbitMq();
        }
    }
}