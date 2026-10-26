using Microsoft.Extensions.Configuration;
using SettlingEngine.Utils;

namespace SettlingEngine.Constants
{
    public class ConfigData
    {
        public static string ConnectionString { get; set; } = string.Empty;
        public static KafkaConfig KafkaConfig { get; set; } = new KafkaConfig();
        public static ServerSettingConfig ServerSettingConfig { get; set; } = new ServerSettingConfig();

        public static void Configure(IConfiguration configuration)
        {
            var AES_Key = configuration["AES_Key"] ?? string.Empty;
            var AES_IV = configuration["AES_InitVector"] ?? string.Empty;
            var connectionStringRaw = configuration["ConnectionString"]?.Trim() ?? string.Empty;

            ConnectionString = EncryptData.DecryptAES(connectionStringRaw, AES_Key, AES_IV);

            var kafkaSection = configuration.GetSection(KafkaConfig.AppsettingsKey);
            if (kafkaSection.Exists())
            {
                kafkaSection.Bind(KafkaConfig);
            }

            var serviceSettingSection = configuration.GetSection(ServerSettingConfig.AppsettingsKey);
            if (serviceSettingSection.Exists())
            {
                serviceSettingSection.Bind(ServerSettingConfig);
            }
        }
    }

    public class KafkaConfig
    {
        public const string AppsettingsKey = "KafkaConfig";

        public bool KafkaAuth { get; init; } = false;
        public string KafkaIP { get; init; } = string.Empty;
        public string KafkaPort { get; init; } = string.Empty;
        public string KafkaUser { get; init; } = string.Empty;
        public string KafkaPassword { get; init; } = string.Empty;
        public string KafkaCALocation { get; init; } = string.Empty;
        public string Acks { get; init; } = "-1"; //mặc định Acks.All
        public string CompressionType { get; init; } = "0"; //mặc định CompressionType.None
        public double LingerMs { get; init; } = 5;
        public int BatchSize { get; init; } = 1000000;
        public string KafkaTopic_SendOrder { get; init; } = string.Empty;
        public string KafkaTopic_Event { get; init; } = string.Empty;
        public string KafkaConsumerGrpID { get; init; } = string.Empty;
        public string KafkaFolderLogSend { get; init; } = string.Empty;
        public string KafkaFolderLogReceive { get; init; } = string.Empty;
    }

    public class ServerSettingConfig
    {
        public const string AppsettingsKey = "ServerSettings";

        public bool UseHttps { get; init; } = false;
        public string Port { get; init; } = string.Empty;
    }
}
