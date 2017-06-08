using MarketParserNet.Framework.Interface;
using MarketParserNet.Utils;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    /// Конфигуратор сервиса из файла
    /// </summary>
    public class FileConfigurationSpiderService : ISpiderServiceConfig
    {
        /// <summary>
        ///  Интервал сколько будет пропущено записей до сохранения в хранилище информации
        /// </summary>
        public int RecordingIntervals => ConfigurationHelper.AppSettingInt("SpiderRecordingIntervals", 50);

        /// <summary>
        /// Начальная ссылка для парсинга
        /// </summary>
        public string StartLink => ConfigurationHelper.AppSetting("SpiderStartLink");

        /// <summary>
        /// Очередь для получения ссылок
        /// </summary>
        public string QueueLinkPath => ConfigurationHelper.AppSetting("SpiderStartLink");
    }
}