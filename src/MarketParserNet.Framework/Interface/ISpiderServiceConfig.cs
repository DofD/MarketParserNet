namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Конфигурация сервиса паука
    /// </summary>
    public interface ISpiderServiceConfig
    {
        /// <summary>
        ///     Интервал сколько будет пропущено записей до сохранения в хранилище информации
        /// </summary>
        int RecordingIntervals { get; }

        /// <summary>
        ///     Начальная ссылка для парсинга
        /// </summary>
        string StartLink { get; }

        /// <summary>
        ///     Очередь для получения ссылок
        /// </summary>
        string QueueLinkPath { get; }

        /// <summary>
        ///     Очередь со страницами для парсинга
        /// </summary>
        string QueueParsePage { get; }
    }
}