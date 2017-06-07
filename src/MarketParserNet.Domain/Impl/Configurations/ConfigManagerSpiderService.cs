using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    /// Конфигуратор сервиса паука
    /// </summary>
    public class ConfigManagerSpiderService: IConfigManager<ISpiderServiceConfig>
    {
        /// <summary>
        ///     Получить конфигурацию
        /// </summary>
        /// <returns>Конфигурация</returns>
        public ISpiderServiceConfig GetConfiguration()
        {
            return FileConfigurationSpiderService();
        }
    }
}