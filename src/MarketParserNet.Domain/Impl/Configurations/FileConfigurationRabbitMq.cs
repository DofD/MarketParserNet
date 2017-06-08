using MarketParserNet.Framework.Interface;
using MarketParserNet.Utils;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    ///     ������������ ������� app.config
    /// </summary>
    public class FileConfigurationRabbitMq : IConfigurationRabbitMq
    {
        // TODO �������� ������������ ������� � ��������� ������

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ����������� �����������������
        /// </summary>
        public const string AsParallelKey = "RabbitMqConfigAsParallel";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ����� �������� �����������
        /// </summary>
        public const string ConsumerRecreateTimeKey = "RabbitMqConfigConsumerRecreateTime";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ��� �����
        /// </summary>
        public const string HostNameKey = "RabbitMqConfigHostName";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ������
        /// </summary>
        public const string PasswordKey = "RabbitMqConfigPassword";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ������������ ���-�� ��������� ��� �������������
        /// </summary>
        public const string PrefetchCountKey = "RabbitMqConfigPrefetchCount";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� �������� ��� ������������
        /// </summary>
        public const string ReconnectIntervalKey = "RabbitMqConfigReconnectInterval";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ��� ������������
        /// </summary>
        public const string UserNameKey = "RabbitMqConfigUserName";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ��� ����� ����������
        /// </summary>
        public const string ExchangeNameKey = "RabbitMqConfigExchangeName";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ����������� ����� ����������
        /// </summary>
        public const string ExchangeDurableKey = "RabbitMqConfigExchangeDurable";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ����������� �������
        /// </summary>
        public const string QueueDurableKey = "RabbitMqConfigQueueDurable";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� �������������� �������
        /// </summary>
        public const string QueueExclusiveKey = "RabbitMqConfigQueueExclusive";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� �������������� �������� �������
        /// </summary>
        public const string QueueAutoDeleteKey = "RabbitMqConfigQueueAutoDelete";

        /// <summary>
        ///     ��������� �����������
        /// </summary>
        public bool AsParallel => ConfigurationHelper.AppSettingBool(AsParallelKey, true);

        /// <summary>
        ///     �������� ������������ �����������, ���� �� ��������� ������ ������� �� ���� �������� ���������
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
        ///     ����
        /// </summary>
        public string HostName => ConfigurationHelper.AppSetting(HostNameKey);

        /// <summary>
        ///     ������ ������������
        /// </summary>
        public string Password => ConfigurationHelper.AppSetting(PasswordKey);

        /// <summary>
        ///     ������������ ���-�� ��������� ��� �������������
        /// </summary>
        public ushort PrefetchCount => (ushort)ConfigurationHelper.AppSettingInt(PrefetchCountKey, 1);

        /// <summary>
        ///     �������� ���������������
        /// </summary>
        public int ReconnectInterval => ConfigurationHelper.AppSettingInt(ReconnectIntervalKey);

        /// <summary>
        ///     ��� ����� ������
        /// </summary>
        public string ExchangeName => ConfigurationHelper.AppSetting(ExchangeNameKey, "MarketParserNet");

        /// <summary>
        ///     ���������� ����� ������
        /// </summary>
        public bool ExchangeDurable => ConfigurationHelper.AppSettingBool(ExchangeDurableKey, true);

        /// <summary>
        ///     ����������� �������
        /// </summary>
        public bool QueueDurable => ConfigurationHelper.AppSettingBool(QueueDurableKey, true);

        /// <summary>
        ///     �������������� �������
        /// </summary>
        public bool QueueExclusive => ConfigurationHelper.AppSettingBool(QueueExclusiveKey);

        /// <summary>
        ///     ���� �������� �������
        /// </summary>
        public bool QueueAutoDelete => ConfigurationHelper.AppSettingBool(QueueAutoDeleteKey);

        /// <summary>
        ///     ��� ������������
        /// </summary>
        public string UserName => ConfigurationHelper.AppSetting(UserNameKey);
    }
}