using DesktopLearningAssistant.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.SettingsWindow
{
    public class SettingsWindowVM
    {
        private ConfigService configService;

        public List<string> SoftWareList { get; set; }

        public List<string> TypeList { get; set; }

        public SettingsWindowVM()
        {
            configService = ConfigService.GetConfigService();

            LoadSoftWareTypeModel();
        }

        private void LoadSoftWareTypeModel()
        {
            SoftWareList = configService.TSConfig.TypeDict.Keys.ToList();
            TypeList = configService.TSConfig.TypeList;
        }
    }
}
