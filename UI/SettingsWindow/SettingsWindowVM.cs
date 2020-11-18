using DesktopLearningAssistant.Configuration;
using DesktopLearningAssistant.TimeStatistic;
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

        public List<string> KeyList { get; set; }

        public List<string> TypeList { get; set; }

        public List<Software> ValueList { get; set; }

        public SettingsWindowVM()
        {
            configService = ConfigService.GetConfigService();

            KeyList = new List<string>();
            TypeList = new List<string>();
            ValueList = new List<Software>();

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            KeyList = configService.TSConfig.TypeDict.Keys.ToList();
            TypeList = configService.TSConfig.TypeList;

            ValueList.Clear();
            TimeStatisticService tss = TimeStatisticService.GetTimeStatisticService();
            List<string> softwareNames = tss.GetSoftwareNames();

            foreach (string softwareName in softwareNames)
            {
                Software software = new Software();
                software.Name = softwareName;
                software.IsChecked = false;
                ValueList.Add(software);
            }
        }
    }

    /// <summary>
    /// 白名单下方所有软件的Model
    /// </summary>
    public class Software
    {
        public string Name { get; set; }

        public bool IsChecked { get; set; }
    }
}
