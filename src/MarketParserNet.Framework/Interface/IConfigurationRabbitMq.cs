namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Интерфейс конфигурации очереди
    /// </summary>
    public interface IConfigurationRabbitMq
    {
        /// <summary>
        ///     Выполнять параллельно
        /// </summary>
        bool AsParallel { get; }

        /// <summary>
        ///     Хост
        /// </summary>
        string HostName { get; }

        /// <summary>
        ///     пароль пользователя
        /// </summary>
        string Password { get; }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     Интервал пересоздания потребителя, если за указанный период времени не было получено сообщение
        /// </summary>
        int ConsumerRecreateTime { get; }

        /// <summary>
        ///     Ограничение на одновременный прием сообщений
        /// </summary>
        /// <remarks>0 - Без ограничений</remarks>
        ushort PrefetchCount { get; }

        /// <summary>
        ///     Интервал переподключения
        /// </summary>
        int ReconnectInterval { get; }
    }
}