using DesktopLearningAssistant.Configuration;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UI.SettingsWindow;

namespace UI
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : WindowX
    {
        private readonly SettingsWindowVM viewModel;

        public Settings()
        {
            InitializeComponent();

            viewModel = new SettingsWindowVM();
            this.DataContext = viewModel;

            RefreshControls();
        }

        private void RefreshControls()
        {
            WhiteListKeyComboBox.Items.Refresh();
            WhiteListValueListView.Items.Refresh();
            TypeKeyComboBox.Items.Refresh();
            TypeValueComboBox.Items.Refresh();
        }

        private void TypeSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            string software = TypeKeyComboBox.SelectedItem.ToString();
            string type = TypeValueComboBox.SelectedItem.ToString();
            ConfigService configService = ConfigService.GetConfigService();
            configService.TSConfig.TypeDict[software] = type;
        }

        private void SoftWareSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string software = TypeKeyComboBox.SelectedItem.ToString();
            string type = ConfigService.GetConfigService().TSConfig.TypeDict[software];
            TypeValueComboBox.SelectedItem = type;
        }

        private void SoftwareListView_Initialized(object sender, EventArgs e)
        {
            
        }

        private void AddWhiteListBtn_Click(object sender, RoutedEventArgs e)
        {
            NewWhiteList newWhiteList = new NewWhiteList();
            if ((bool)newWhiteList.ShowDialog())
            {
                string newWhiteListName = newWhiteList.WhiteListName;
                ConfigService configService = ConfigService.GetConfigService();
                if (configService.TTConfig.WhiteLists.ContainsKey(newWhiteListName)) return;

                // 更新数据
                configService.TTConfig.WhiteLists.Add(newWhiteListName, new List<string>());
                viewModel.UpdateWhiteListKey();
                viewModel.UpdateWhiteListValue(newWhiteList.WhiteListName);

                // 更新控件
                WhiteListKeyComboBox.ItemsSource = viewModel.WhiteListKeyList;
                WhiteListKeyComboBox.SelectedIndex = WhiteListKeyComboBox.Items.Count - 1;  // 选择新的
                WhiteListKeyComboBox.Items.Refresh();
                WhiteListValueListView.Items.Refresh();
            }

            MessageBox.Show("添加成功");
        }

        private void DeleteWhiteListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WhiteListKeyComboBox.SelectedItem == null)
                return;
            string selectedKey = WhiteListKeyComboBox.SelectedItem.ToString();
            ConfigService configService = ConfigService.GetConfigService();

            // 更新数据
            configService.TTConfig.WhiteLists.Remove(selectedKey);
            viewModel.UpdateWhiteListKey();
            viewModel.UpdateWhiteListValue();

            // 刷新控件
            WhiteListKeyComboBox.ItemsSource = viewModel.WhiteListKeyList;
            WhiteListKeyComboBox.Items.Refresh();
            WhiteListValueListView.Items.Refresh();

            MessageBox.Show("删除成功");
        }

        private void SaveWhiteListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WhiteListKeyComboBox.SelectedItem == null)
                return;
            string selectedKey = WhiteListKeyComboBox.SelectedItem.ToString();
            List<string> selectedValues = new List<string>();
            foreach (Software software in viewModel.WhiteListValueList)
            {
                if (software.IsChecked) selectedValues.Add(software.Name);
            }

            if (selectedKey == "") return;

            ConfigService configService = ConfigService.GetConfigService();
            configService.TTConfig.WhiteLists[selectedKey] = selectedValues;        // 更新专注白名单

            MessageBox.Show("保存成功");
        }

        private void WhiteListKey_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WhiteListKeyComboBox.SelectedItem == null) return;
            string selectedKey = WhiteListKeyComboBox.SelectedItem.ToString();

            // 更新数据
            viewModel.UpdateWhiteListValue(selectedKey);

            // 刷新控件
            WhiteListValueListView.Items.Refresh();
        }
    }
}
