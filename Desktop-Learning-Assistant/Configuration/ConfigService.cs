using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopLearningAssistant.Configuration.Config;

namespace DesktopLearningAssistant.Configuration
{
    public class ConfigService
    {
        public TimeStatisticConfig TSConfig { get; set; }
        private static ConfigService uniqueConfigService;

        public ConfigService()
        {
            //TODO 从配置文件中读出配置
        }

        public static ConfigService GetConfigService()
        {
            if (uniqueConfigService == null)
            {
                lock(uniqueConfigService)
                {
                    uniqueConfigService = new ConfigService();
                }
            }
            return uniqueConfigService;
        }
    }
}
