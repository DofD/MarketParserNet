using MarketParserNet.Framework.Interface;
using MarketParserNet.Utils;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    ///     Конфигурация очереди app.config
    /// </summary>
    public class FileConfigurationRabbitMq : IConfigurationRabbitMq
    {
        // TODO выделить конфигурацию очереди в отдельную секцию

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за возможность распараллеливания
        /// </summary>
        public const string AsParallelKey = "RabbitMqConfigAsParallel";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за время проверки потребителя
        /// </summary>
        public const string ConsumerRecreateTimeKey = "RabbitMqConfigConsumerRecreateTime";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за имя хоста
        /// </summary>
        public const string HostNameKey = "RabbitMqConfigHostName";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за пароль
        /// </summary>
        public const string PasswordKey = "RabbitMqConfigPassword";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за Максимальное кол-во сообщений без подтверждения
        /// </summary>
        public const string PrefetchCountKey = "RabbitMqConfigPrefetchCount";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за интервал при переключении
        /// </summary>
        public const string ReconnectIntervalKey = "RabbitMqConfigReconnectInterval";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за имя пользователя
        /// </summary>
        public const string UserNameKey = "RabbitMqConfigUserName";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за имя точки расширения
        /// </summary>
        public const string ExchangeNameKey = "RabbitMqConfigExchangeName";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за постоянство точки расширения
        /// </summary>
        public const string ExchangeDurableKey = "RabbitMqConfigExchangeDurable";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за постоянство очереди
        /// </summary>
        public const string QueueDurableKey = "RabbitMqConfigQueueDurable";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за эксклюзивность очереди
        /// </summary>
        public const string QueueExclusiveKey = "RabbitMqConfigQueueExclusive";

        /// <summary>
        ///     Имя ключа в конфигурации отвечающее за автоматическое удаление очереди
        /// </summary>
        public const string QueueAutoDeleteKey = "RabbitMqConfigQueueAutoDelete";

        /// <summary>
        ///     Выполнять параллельно
        /// </summary>
        public bool AsParallel => ConfigurationHelper.AppSettingBool(AsParallelKey, true);

        /// <summary>
        ///     Интервал пересоздания потребителя, если за указанный период времени не было получено сообщение
        /// </summary>
        public int ConsumerRecreateTime
        {
            get
            {
                var val = ConfigurationHelper.AppSettingInt(ConsumerRecreateTimeKey);
                return val == 0 ? 60000 : val;
            }
        }

        /// <summary>
        ///     Хост
        /// </summary>
        public string HostName => ConfigurationHelper.AppSetting(HostNameKey);

        /// <summary>
        ///     пароль пользователя
        /// </summary>
        public string Password => ConfigurationHelper.AppSetting(PasswordKey);

        /// <summary>
        ///     Максимальное кол-во сообщений без подтверждения
        /// </summary>
        public ushort PrefetchCount => (ushort)ConfigurationHelper.AppSettingInt(PrefetchCountKey, 1);

        /// <summary>
        ///     Интервал переподключения
        /// </summary>
        public int ReconnectInterval => ConfigurationHelper.AppSettingInt(ReconnectIntervalKey);

        /// <summary>
        ///     Имя точки обмена
        /// </summary>
        public string ExchangeName => ConfigurationHelper.AppSetting(ExchangeNameKey, "MarketParserNet");

        /// <summary>
        ///     Постоянная точка обмена
        /// </summary>
        public bool ExchangeDurable => ConfigurationHelper.AppSettingBool(ExchangeDurableKey, true);

        /// <summary>
        ///     Постоянство очереди
        /// </summary>
        public bool QueueDurable => ConfigurationHelper.AppSettingBool(QueueDurableKey, true);

        /// <summary>
        ///     Эксклюзивность очереди
        /// </summary>
        public bool QueueExclusive => ConfigurationHelper.AppSettingBool(QueueExclusiveKey);

        /// <summary>
        ///     Авто удаление очереди
        /// </summary>
        public bool QueueAutoDelete => ConfigurationHelper.AppSettingBool(QueueAutoDeleteKey);

        /// <summary>
        ///     Имя пользователя
        /// </summary>
        public string UserName => ConfigurationHelper.AppSetting(UserNameKey);
    }
}