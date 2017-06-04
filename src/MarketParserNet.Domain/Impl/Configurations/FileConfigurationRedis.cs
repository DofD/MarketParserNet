using MarketParserNet.Framework.Interface;
using MarketParserNet.Utils;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    ///     ������������ Redis
    /// </summary>
    public class FileConfigurationRedis : IRedisConfig
    {
        // TODO �������� ������������ Redis � ��������� ������

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ������ ����������� � Redis
        /// </summary>
        public const string ConnectionStringKey = "RedisConnectionString";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ���������� ������� ����������� � Redis
        /// </summary>
        public const string RetryCountKey = "RedisRetryCount";

        /// <summary>
        ///     ��� ����� � ������������ ���������� �� ����� ����� ��������� ����������� � Redis
        /// </summary>
        public const string ReconnectIntervalKey = "RedisReconnectInterval";

        /// <summary>
        ///     ������ �����������
        /// </summary>
        public string ConnectionString => ConfigurationHelper.AppSetting(ConnectionStringKey, "localhost");

        /// <summary>
        ///     ���������� ������� �����������
        /// </summary>
        public int RetryCount => ConfigurationHelper.AppSettingInt(RetryCountKey, 1);

        /// <summary>
        ///     �������� ���������������
        /// </summary>
        public int ReconnectInterval => ConfigurationHelper.AppSettingInt(ReconnectIntervalKey, 30000);
    }
}