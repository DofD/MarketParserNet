using MarketParserNet.Framework.Interface;
using MarketParserNet.Utils;

namespace MarketParserNet.Domain.Impl.Configurations
{
    /// <summary>
    /// ������������ ������� �� �����
    /// </summary>
    public class FileConfigurationSpiderService : ISpiderServiceConfig
    {
        /// <summary>
        ///  �������� ������� ����� ��������� ������� �� ���������� � ��������� ����������
        /// </summary>
        public int RecordingIntervals => ConfigurationHelper.AppSettingInt("SpiderRecordingIntervals", 50);

        /// <summary>
        /// ��������� ������ ��� ��������
        /// </summary>
        public string StartLink => ConfigurationHelper.AppSetting("SpiderStartLink");

        /// <summary>
        /// ������� ��� ��������� ������
        /// </summary>
        public string QueueLinkPath => ConfigurationHelper.AppSetting("SpiderStartLink");
    }
}