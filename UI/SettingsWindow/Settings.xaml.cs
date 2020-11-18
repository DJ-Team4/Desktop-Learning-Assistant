using DesktopLearningAssistant.Configuration;
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
using DesktopLearningAssistant.TimeStatistic;
using DesktopLearningAssistant.TimeStatistic.Model;

namespace UI
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Window
    {
        private SettingsWindowVM viewModel;

        public Settings()
        {
            InitializeComponent();

            viewModel = new SettingsWindowVM();
            this.DataContext = viewModel;
        }

        private void TypeSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            string software = SoftWareComboBox.SelectedItem.ToString();
            string type = TypeComboBox.SelectedItem.ToString();
            ConfigService configService = ConfigService.GetConfigService();
            configService.TSConfig.TypeDict[software] = type;
        }

        private void SoftWareSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string software = SoftWareComboBox.SelectedItem.ToString();
            string type = ConfigService.GetConfigService().TSConfig.TypeDict[software];
            TypeComboBox.SelectedItem = type;
        }

        private void SoftwareListView_Initialized(object sender, EventArgs e)
        {
            
        }

        private void AddWhiteListBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteWhiteListBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
