namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    /// Конфигурация сервиса парсинга
    /// </summary>
    public interface IParserServiceConfig
    {
        /// <summary>
        ///     Очередь со страницами для парсинга
        /// </summary>
        string QueueParsePage { get; }
    }
}