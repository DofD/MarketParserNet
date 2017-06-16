namespace MarketParserNet.Domain.Impl.Configurations
{
    using Framework.Interface;

    using Utils;

    public class FileConfigurationParserService : IParserServiceConfig
    {
        /// <summary>
        ///     Очередь со страницами для парсинга
        /// </summary>
        public string QueueParsePage => ConfigurationHelper.AppSetting("ParserQueueParsePage");
    }
}