using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    /// Конфигурация RabbitMq
    /// </summary>
    public class ConfigurationRabbitMq : IConfigurationRabbitMq
    {
        /// <summary>
        ///     Выполнять параллельно
        /// </summary>
        public bool AsParallel { get; }

        /// <summary>
        ///     Хост
        /// </summary>
        public string HostName { get; }

        /// <summary>
        ///     пароль пользователя
        /// </summary>
        public string Password { get; }

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string UserName { get; }

        /// <summary>
        ///     Интервал пересоздания потребителя, если за указанный период времени не было получено сообщение
        /// </summary>
        public int ConsumerRecreateTime { get; }

        /// <summary>
        ///     Ограничение на одновременный прием сообщений
        /// </summary>
        /// <remarks>0 - Без ограничений</remarks>
        public ushort PrefetchCount { get; }

        /// <summary>
        ///     Интервал переподключения
        /// </summary>
        public int ReconnectInterval { get; }

        /// <summary>
        ///     Имя точки обмена
        /// </summary>
        public string ExchangeName { get; }

        /// <summary>
        ///     Постоянная точка обмена
        /// </summary>
        public bool ExchangeDurable { get; }

        /// <summary>
        ///     Постоянство очереди
        /// </summary>
        public bool QueueDurable { get; }

        /// <summary>
        ///     Эксклюзивность очереди
        /// </summary>
        public bool QueueExclusive { get; }

        /// <summary>
        ///     Авто удаление очереди
        /// </summary>
        public bool QueueAutoDelete { get; }
    }
}