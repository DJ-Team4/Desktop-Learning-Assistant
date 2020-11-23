using DesktopLearningAssistant.Configuration;
using DesktopLearningAssistant.TimeStatistic;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace UI.SettingsWindow
{
    public class SettingsWindowVM
    {
        private ConfigService configService;

        public List<string> TypeKeyList { get; set; }

        public List<string> TypeValueList { get; set; }

        public List<string> WhiteListKeyList { get; set; }

        public List<Software> WhiteListValueList { get; set; }

        public SettingsWindowVM()
        {
            configService = ConfigService.GetConfigService();

            TypeKeyList = new List<string>();
            TypeValueList = new List<string>();
            WhiteListKeyList = new List<string>();
            WhiteListValueList = new List<Software>();

            InitializeViewModel();
        }

        public void InitializeViewModel()
        {
            UpdateWhiteListKey();
            UpdateWhiteListValue();
            UpdateTypeKey();
            UpdateTypeValue();
        }

        public void UpdateWhiteListKey()
        {
            WhiteListKeyList.Clear();
            WhiteListKeyList = configService.TTConfig.WhiteLists.Keys.ToList();
        }

        public void UpdateWhiteListValue(string selectedWhiteListKey = null)
        {
            WhiteListValueList.Clear();
            TimeStatisticService tss = TimeStatisticService.GetTimeStatisticService();
            List<string> softwareNames = tss.GetSoftwareNames();

            foreach (string softwareName in softwareNames)
            {
                Software software = new Software();
                software.Name = softwareName;
                if (selectedWhiteListKey != null && configService.TTConfig.WhiteLists[selectedWhiteListKey].Contains(softwareName))
                {
                    software.IsChecked = true;
                }
                else
                {
                    software.IsChecked = false;
                }

                WhiteListValueList.Add(software);
            }
        }

        public void UpdateTypeKey()
        {
            TypeKeyList = configService.TSConfig.TypeDict.Keys.ToList();
        }

        public void UpdateTypeValue()
        {
            TypeValueList = configService.TSConfig.TypeList;
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
